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

            static const int permutation_table[257] = {
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

            static float fade(const float t)
            {
                return t * t * t * (t * (t * 6 - 15) + 10);
            }

            static float gradient_value(const int hash, const float x, const float y)
            {
                return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
            }

            static float perlin_noise(float ix, float iy)
            {
                int x = (int)floor(ix) & 0xff;
                const int y = (int)floor(iy) & 0xff;
                ix -= floor(ix);
                iy -= floor(iy);
                const float u = fade(ix);
                const float v = fade(iy);
                int a = (permutation_table[x] + y) & 0xff;
                int b = (permutation_table[x + 1] + y) & 0xff;

                return lerp(
                    lerp(
                        gradient_value(permutation_table[a], ix, iy),
                        gradient_value(permutation_table[b], ix - 1, iy),
                        u
                    ),
                    lerp(
                        gradient_value(permutation_table[a + 1], ix, iy - 1),
                        gradient_value(permutation_table[b + 1], ix - 1, iy - 1),
                        u
                    ),
                    v
                );
            }

            static float perlin_noise(float2 pos) { return perlin_noise(pos.x, pos.y); }

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                v.vertex.xy *= lerp(1, 0.1, fixed2(_WarpX, _WarpY));

                float2 offset = float2(_OffsetX, _OffsetY);
                float2 pt = v.vertex.xy + offset;

                fixed mask = saturate(perlin_noise(pt * 0.125 - _OffsetSeed.xy + offset * 0.2) + 0.6);
                mask = saturate(pow(mask, 5 - _Reach));

                fixed perlin = saturate((
                    perlin_noise(pt * 0.25 + _OffsetSeed.xy + offset * 0.2) +
                    perlin_noise(pt * 0.5 + _OffsetSeed.zw + offset * 0.1) * 0.5 +
                    perlin_noise(pt + _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.05) * 0.2
                ) * 0.6 + 0.5);
                o.color = lerp(0, _Color, perlin * mask) * _Opacity;

                offset *= 1.7;
                pt = v.vertex.xy + offset;
                perlin = (
                    perlin_noise(pt * 0.25 + _OffsetSeed.xy - _OffsetSeed.zw + offset * 0.2) +
                    perlin_noise(pt * 0.5 - _OffsetSeed.xy + _OffsetSeed.zw + offset * 0.1) * 0.5 +
                    perlin_noise(pt - _OffsetSeed.zw + offset * 0.05) * 0.2
                ) * 0.6 + 0.5;
                perlin *= perlin;
                o.color2 = lerp(0, _Color2, perlin * mask) * _Opacity;

                return o;
            }

            fixed4 frag(const v2f i) : COLOR
            {
                return 1 - (1 - i.color) * (1 - i.color2);
            }
            ENDCG
        }
    }
}