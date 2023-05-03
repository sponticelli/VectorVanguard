/* The edge detection algorithm that is implemented in this shader is named "Sobel Edge Detection" */
Shader "LiteNinja/MeshCraft/Edge Detection"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }
        Cull Back

        GrabPass {}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : POSITION;
                float4 screenPos : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _GrabTexture : register(s0);

            v2f vert(const appdata input)
            {
                v2f output;

                output.position = UnityObjectToClipPos(input.vertex);
                output.screenPos = output.position;

                return output;
            }

            half4 pixel;
            half2 uv;
            fixed one_pixel_w, one_pixel_h;

            half4 frag(v2f input) : SV_Target
            {
                uv = input.screenPos.xy / input.screenPos.w;
                uv.x = (uv.x + 1) * .5;
                uv.y = 1.0 - (uv.y + 1) * .5;

                one_pixel_w = 1.0 / _ScreenParams.x;
                one_pixel_h = 1.0 / _ScreenParams.y;

                pixel = 0;
                pixel = abs(
                    tex2D(_GrabTexture, half2(uv.x - one_pixel_w, uv.y)) -
                    tex2D(_GrabTexture, half2(uv.x + one_pixel_w, uv.y)) +
                    tex2D(_GrabTexture, half2(uv.x, uv.y + one_pixel_h)) -
                    tex2D(_GrabTexture, half2(uv.x, uv.y - one_pixel_h))
                );

                return pixel * _Color;
            }
            ENDCG
        }
    }
}