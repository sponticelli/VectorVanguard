Shader "LiteNinja/MeshCraft/Wireframe/Unlit/Transparent/2 Sided/Wire Only"
{
    Properties
    {
        //Base
        [HideInInspector] _Color("", color) = (1, 1, 1, 1)
        [HideInInspector] _MainTex("", 2D) = "white"{}

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
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite Off
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #pragma multi_compile_fog

            #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON
            
            #define MC_TRANSPARENT
            #define MC_NO_COLOR_BLACK
            #define MC_SAME_COLOR

            #include "cginc/Wireframe_Unlit.cginc"
            ENDCG

        } //Pass

        Pass
        {
            ZWrite On
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #pragma multi_compile_fog
            
            #pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON

            #define MC_TRANSPARENT
            #define MC_NO_COLOR_BLACK
            #define MC_SAME_COLOR

            #include "cginc/Wireframe_Unlit.cginc"
            ENDCG

        } //Pass

    } //SubShader

} //Shader