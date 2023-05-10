#ifndef MESHCRAFT_WIREFRAME_STANDRAD_CGINC
#define MESHCRAFT_WIREFRAME_STANDARD_CGINC


#include <UnityShaderVariables.cginc>

#include "./Wireframe_Core.cginc"


inline void SetupWireframe(const float3 mass, float2 wire_tex_uv, float3 world_pos, float3 worldNormal, inout float3 dstColor,
                           inout float alpha, out float3 emission)
{
    const fixed4 wire_tex_color = tex2D(_MC_WireTex, wire_tex_uv);
	
    //Transparency
    #ifdef MC_TRANSPARENCY_ON
		half customAlpha = wireTexColor.a;
		customAlpha = lerp(customAlpha, 1 - customAlpha, _MC_TransparentTex_Invert);
		customAlpha = saturate(customAlpha + _MC_TransparentTex_Alpha_Offset);
		_MC_Color.a *= customAlpha;
    #endif


    const float fixed_size = distance(_WorldSpaceCameraPos, world_pos);
    half4 ret_color = dstColor.rgbb;
	
    WireOpaque(wire_tex_color, ret_color, mass,  fixed_size, emission);
    dstColor = ret_color.rgb;
}


#endif
