Shader "LiteNinja/MeshCraft/Wireframe/Lit/Additive/ZWrite/Wire Only"
{
    Properties
    {
        [MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2

        [HideInInspector] _Color("Color (RGB)", color) = (1, 1, 1, 1)
        [HideInInspector] _MainTex("Base (RGB)", 2D) = "white"{}

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
        UsePass "LiteNinja/MeshCraft/Wireframe/Lit/Additive/Simple/Wire Only/BASE"

    } //SubShader

} //Shader