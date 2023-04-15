Shader "VectorVanguard/Noise"
{
    Properties
    {

        _Color("Color", Color) = (1,1,1,1)
        _Color2("Color 2", Color) = (1,1,1,1)
        [Space()]
        _Opacity("Opacity", Range(0.0, 1.0)) = 1.0
        _WarpX("Warp X", Range(0, 1)) = 0.75
        _WarpY("Warp Y", Range(0, 1)) = 0.75
        _Reach("Reach", Range(0, 5)) = 2
        
        [Space()]
        _OffsetX("Offset X", Float) = 0
        _OffsetY("Offset Y", Float) = 0
        _OffsetSeed("Offset Seed", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Background+1"
            "DisableBatching"="True"
            "ForceNoShadowCasting"="True"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        ZWrite Off
        Cull Back

        Pass
        {
            Blend OneMinusDstColor One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color, _Color2;
            fixed _Opacity;

            fixed _WarpX, _WarpY;
            float _OffsetX, _OffsetY;
            float4 _OffsetSeed;
            float _Reach;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                fixed4 color2 : TEXCOORD0;
            };
            
            static const int _Perm[257] = {
                27, 123, 226, 54, 127, 145, 211, 159, 111, 72, 50, 113, 0, 159, 145, 153, 10, 127, 182, 65, 104, 103,
                112, 209, 93, 184, 82, 234, 143, 144, 250, 144, 25, 189, 130, 129, 18, 204, 11, 225, 92, 220, 29, 199,
                11, 25, 1, 141, 216, 43, 4, 115, 87, 242, 224, 148, 37, 83, 137, 46, 212, 105, 54, 53, 120, 37, 214, 32,
                164, 54, 73, 8, 99, 169, 176, 247, 69, 194, 194, 158, 191, 100, 99, 94, 209, 53, 58, 212, 200, 0, 80,
                176, 185, 237, 110, 163, 183, 116, 88, 214, 229, 67, 83, 80, 187, 75, 194, 221, 24, 121, 13, 29, 145,
                105, 180, 21, 135, 37, 81, 137, 92, 159, 123, 47, 169, 0, 227, 125, 162, 177, 227, 180, 226, 239, 38,
                206, 243, 10, 201, 114, 54, 136, 3, 114, 107, 81, 250, 135, 54, 234, 171, 187, 96, 45, 19, 39, 86, 57,
                193, 8, 215, 110, 73, 191, 21, 198, 103, 71, 96, 180, 65, 222, 213, 137, 222, 228, 153, 58, 235, 145,
                36, 192, 127, 238, 125, 154, 81, 82, 233, 20, 104, 252, 34, 105, 215, 126, 151, 240, 137, 24, 211, 94,
                69, 95, 22, 33, 19, 172, 239, 212, 58, 159, 98, 149, 82, 129, 251, 124, 88, 146, 69, 139, 56, 29, 252,
                193, 51, 133, 237, 242, 25, 154, 20, 104, 30, 214, 153, 163, 82, 21, 75, 18, 208, 26, 43, 247, 128, 62,
                207, 17, 181, 242, 91, 232, 1, 232, 45
            };

            static float Fade(float _t)
            {
                return _t * _t * _t * (_t * (_t * 6 - 15) + 10);
            }

            static float Grad(int _hash, float _x, float _y)
            {
                return ((_hash & 1) == 0 ? _x : -_x) + ((_hash & 2) == 0 ? _y : -_y);
            }

            static float Grad(int _hash, float _x, float _y, float _z)
            {
                int h = _hash & 15;
                float u = h < 8 ? _x : _y;
                float v = h < 4 ? _y : (h == 12 || h == 14 ? _x : _z);
                return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
            }

            static float Noise(float _x, float _y)
            {
                int X = (int)floor(_x) & 0xff;
                int Y = (int)floor(_y) & 0xff;
                _x -= floor(_x);
                _y -= floor(_y);
                float u = Fade(_x);
                float v = Fade(_y);
                int A = (_Perm[X] + Y) & 0xff;
                int B = (_Perm[X + 1] + Y) & 0xff;

                return lerp(
                    lerp(
                        Grad(_Perm[A], _x, _y),
                        Grad(_Perm[B], _x - 1, _y),
                        u
                    ),
                    lerp(
                        Grad(_Perm[A + 1], _x, _y - 1),
                        Grad(_Perm[B + 1], _x - 1, _y - 1),
                        u
                    ),
                    v
                );
            }

            static float Noise(float2 _pos) { return Noise(_pos.x, _pos.y); }

            static float Noise(float x, float y, float z)
            {
                int X = (int)floor(x) & 0xff;
                int Y = (int)floor(y) & 0xff;
                int Z = (int)floor(z) & 0xff;
                x -= floor(x);
                y -= floor(y);
                z -= floor(z);
                const float u = Fade(x);
                const float v = Fade(y);
                const float w = Fade(z);
                int a = (_Perm[X] + Y) & 0xff;
                int b = (_Perm[X + 1] + Y) & 0xff;
                int aa = (_Perm[a] + Z) & 0xff;
                int ba = (_Perm[b] + Z) & 0xff;
                int ab = (_Perm[a + 1] + Z) & 0xff;
                int bb = (_Perm[b + 1] + Z) & 0xff;

                return lerp(
                    lerp(lerp(Grad(_Perm[aa], x, y, z), Grad(_Perm[ba], x - 1, y, z), u),
                         lerp(Grad(_Perm[ab], x, y - 1, z), Grad(_Perm[bb], x - 1, y - 1, z), u), v),
                    lerp(lerp(Grad(_Perm[aa + 1], x, y, z - 1), Grad(_Perm[ba + 1], x - 1, y, z - 1), u),
                         lerp(Grad(_Perm[ab + 1], x, y - 1, z - 1), Grad(_Perm[bb + 1], x - 1, y - 1, z - 1), u), v),
                    w
                );
            }


            

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                v.vertex.xy *= lerp(1, 0.1, fixed2(_WarpX, _WarpY));

                float2 offset = float2(_OffsetX, _OffsetY);
                float2 pt = v.vertex.xy + offset;

                fixed mask = saturate(Noise(pt * 0.125 - _OffsetSeed.xy + offset * 0.2) + 0.6);
                mask = saturate(pow(mask, 5 - _Reach));

                fixed perlin = saturate((
                    Noise(pt * 0.25 + _OffsetSeed.xy + offset * 0.2) +
                    Noise(pt * 0.5 + _OffsetSeed.zw + offset * 0.1) * 0.5 +
                    Noise(pt + _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.05) * 0.2
                ) * 0.6 + 0.5);
                o.color = lerp(0, _Color, perlin * mask) * _Opacity;

                offset *= 1.7;
                pt = v.vertex.xy + offset;
                perlin = (
                    Noise(pt * 0.25 + _OffsetSeed.xy - _OffsetSeed.zw + offset * 0.2) +
                    Noise(pt * 0.5 - _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.1) * 0.5 +
                    Noise(pt - _OffsetSeed.zw + offset * 0.05) * 0.2
                ) * 0.6 + 0.5;
                perlin *= perlin;
                o.color2 = lerp(0, _Color2, perlin * mask) * _Opacity;


                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                return 1 - (1 - i.color) * (1 - i.color2);
            }
            ENDCG
        }
    }
}