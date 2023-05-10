Shader "LiteNinja/MeshCraft/Wireframe/Lit/Transparent/ZWrite/Diffuse"
{
    Properties
    {
        [MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2

        //Visual Options	 
        [MC_Label] _MC_Label_V_Options("Default Visual Options", float) = 0

        //Base 
        _Color("Color (RGB) Trans (A)", color) = (1, 1, 1, 1)

        //Vertex Color
        [MC_Toggle] _MC_VertexColor("Vertex Color", float) = 0

        _MainTex("Base (RGB) Trans(A)", 2D) = "white"{}
        [MC_UVScroll] _MC_MainTex_Scroll("    ", vector) = (0, 0, 0, 0)

        //Bump
        [MC_BumpODL] _MC_BumpEnumID ("", Float) = 0
        [HideInInspector] _MC_NormalMap ("", 2D) = "bump" {}

        //Specular
        [MC_Specular] _MC_SpecularEnumID ("", Float) = 0
        [HideInInspector] _MC_Specular_Lookup("", 2D) = "black"{}

        //Reflection
        [MC_Reflection] _MC_ReflectionEnumID("", float) = 0
        [HideInInspector] _Cube("", Cube) = ""{}
        [HideInInspector] _ReflectColor("", Color) = (0.5, 0.5, 0.5, 1)
        [HideInInspector] _MC_Reflection_Strength("", Range(0, 1)) = 0.5
        [HideInInspector] _MC_Reflection_Fresnel_Bias("", Range(-1, 1)) = -1
        [HideInInspector] _MC_Reflection_Roughness("", Range(0, 1)) = 0.3

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

        //Improved Transparent Blend
        [MC_ImprovedBlend] _MC_ImprovedBlendEnumID ("", int) = 0

        //Transparency          
        [MC_Label] _MC_Transparency_M_Options("Wire Transparency Options", float) = 0

        [MC_Transparency] _MC_TransparencyEnumID("", float) = 0
        [HideInInspector] _MC_TransparentTex_Invert("    ", float) = 0
        [HideInInspector] _MC_TransparentTex_Alpha_Offset("    ", Range(-1, 1)) = 0


        
    }


    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 200
        Cull [_Cull]
        Blend SrcAlpha OneMinusSrcAlpha

        UsePass "Hidden/LiteNinja/MeshCraft/Wireframe/ColorMask0 NoInstance/BASE"

        //PassName "FORWARD" 
        Pass
        {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nodirlightmap nodynlightmap
            #pragma target 3.0

            #pragma multi_compile_fog
            #pragma shader_feature MC_REFLECTION_OFF MC_REFLECTION_CUBE_SIMPLE MC_REFLECTION_CUBE_ADVANED MC_REFLECTION_UNITY_REFLECTION_PROBES


            #define MC_HAS_TEXTURE
            #define MC_TRANSPARENT
            #define MC_NO

            #include "cginc/Wireframe_ForwardBase.cginc"
            ENDCG
        } //Pass   	

        Pass
        {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nodirlightmap nodynlightmap
            #pragma target 3.0

            #pragma multi_compile_fog


            #pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
            #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON

            #define MC_TRANSPARENT
            #define MC_NO_COLOR_BLACK 
            #define MC_SAME_COLOR 

            #include "./cginc/Wireframe_ForwardBase.cginc"
            ENDCG
        } //Pass   	

    } //SubShader

    FallBack "LiteNinja/MeshCraft/Wireframe/Vertex Lit/Transparent/Full"
} //Shader