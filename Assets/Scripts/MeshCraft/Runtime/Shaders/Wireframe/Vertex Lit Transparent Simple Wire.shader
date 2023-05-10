Shader "LiteNinja/MeshCraft/Wireframe/Vertex Lit/Transparent/Wire Only"
{
    Properties
    {
        //Base 
        [HideInInspector] _Color("Color (RGB) Trans (A)", color) = (1, 1, 1, 1)
        [HideInInspector] _MainTex("Base (RGB) Trans (A)", 2D) = "white"{}

        //Wire Options  
        [MC_Label] _MC_Label_W_Options("Wire Visual Options", float) = 0
        _MC_LineWidth("Line Width", Float) = 1
        _MC_Color("Color", color) = (0, 0, 0, 1)
        _MC_WireTex("Color Texture (RGBA)", 2D) = "white"{}
        [MC_UVScroll] _MC_WireTex_Scroll("    ", vector) = (0, 0, 0, 0)
        [Enum(UV0,0,UV1,1)] _MC_WireTex_UVSet("    UV Set", float) = 0

        //Emission
        [MC_PositiveFloat]_MC_EmissionStrength("Emission Strength", float) = 0

        //Vertex Color
        [MC_Toggle] _MC_WireVertexColor("Vertex Color", Float) = 0

        //Light
        [MC_IncludeLight] _MC_IncludeLightEnumID ("", float) = 0

        //Transparency          
        [MC_Label] _MC_Transparency_M_Options("Wire Transparency Options", float) = 0
        [MC_Transparency] _MC_TransparencyEnumID("", float) = 0
        [HideInInspector] _MC_TransparentTex_Invert("    ", float) = 0
        [HideInInspector] _MC_TransparentTex_Alpha_Offset("    ", Range(-1, 1)) = 0

        
    }

    Category
    {
        Tags
        {
            "Queue"="Transparent+1"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 150

        Blend SrcAlpha OneMinusSrcAlpha

        SubShader
        {

            // Vertex Lit, emulated in shaders (4 lights max, no specular)
            Pass
            {
                Tags
                {
                    "LightMode" = "Vertex"
                }
                Lighting On

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders


                #pragma multi_compile_fog


                #ifdef UNITY_PASS_DEFERRED
					#define MC_LIGHT_ON
                #else
                    #pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
                #endif
                #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


                #define MC_TRANSPARENT
                #define MC_NO_COLOR_BLACK
                #define MC_SAME_COLOR

                #include "cginc/Wireframe_VertexLit.cginc"
                ENDCG
            }

            // Lightmapped
            Pass
            {
                Tags
                {
                    "LightMode" = "VertexLM"
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders


                #pragma multi_compile_fog


                #ifdef UNITY_PASS_DEFERRED
					#define MC_LIGHT_ON
                #else
                    #pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
                #endif
                #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


                #define MC_LIGHTMAP_ON
                #define MC_TRANSPARENT
                #define MC_NO_COLOR_BLACK
                #define MC_SAME_COLOR

                #include "cginc/Wireframe_VertexLit.cginc"
                ENDCG
            }

            // Lightmapped, encoded as RGBM
            Pass
            {
                Tags
                {
                    "LightMode" = "VertexLMRGBM"
                }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders


                #pragma multi_compile_fog


                #ifdef UNITY_PASS_DEFERRED
					#define MC_LIGHT_ON
                #else
                    #pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
                #endif
                #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


                #define MC_LIGHTMAP_ON
                #define MC_TRANSPARENT
                #define MC_NO_COLOR_BLACK
                #define MC_SAME_COLOR

                #include "cginc/Wireframe_VertexLit.cginc"
                ENDCG
            }
        }
    }

    FallBack Off
}