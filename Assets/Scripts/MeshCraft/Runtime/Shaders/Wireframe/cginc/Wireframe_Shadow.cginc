#ifndef MESHCRAFT_WIREFRAME_SHADOW_CGINC
#define MESHCRAFT_WIREFRAME_SHADOW_CGINC


#include "UnityCG.cginc"
#include "./Wireframe_Core.cginc"


struct v2f_surf
{
    V2F_SHADOW_CASTER;
    half3 custompack2 : TEXCOORD1;
    float3 objectPos : TEXCOORD5;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


v2f_surf vert_surf(appdata_full v)
{
    v2f_surf o = (v2f_surf)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.objectPos = v.vertex.xyz;
    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
    return o;
}


float4 frag(v2f_surf i) : SV_Target
{
    SHADOW_CASTER_FRAGMENT(i)
}
#endif
