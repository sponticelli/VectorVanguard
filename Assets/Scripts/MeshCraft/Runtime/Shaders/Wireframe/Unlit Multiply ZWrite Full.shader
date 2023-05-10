Shader "LiteNinja/MeshCraft/Wireframe/Unlit/Multiply/ZWrite/Full"
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

        _MainTex("Base (RGB) Trans (A)", 2D) = "white"{}
        [MC_UVScroll] _MC_MainTex_Scroll("    ", vector) = (0, 0, 0, 0)


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
            "Queue"="Transparent+2"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull [_Cull]


        UsePass "Hidden/LiteNinja/MeshCraft/Wireframe/ColorMask0 NoInstance/BASE"


        Blend Zero SrcColor
        ColorMask RGB
        Lighting Off ZWrite Off Fog
        {
            Color (0,0,0,0)
        }
        Cull [_Cull]


        Pass
        {
            Name "BASE"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile_fog

            #pragma shader_feature MC_REFLECTION_OFF MC_REFLECTION_CUBE_SIMPLE MC_REFLECTION_CUBE_ADVANED MC_REFLECTION_UNITY_REFLECTION_PROBES
            #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


            #define MC_HAS_TEXTURE		
            #define MC_MULTIPLY

            #include "cginc/Wireframe_Unlit.cginc"
            ENDCG

        } //Pass

    } //SubShader

} //Shader