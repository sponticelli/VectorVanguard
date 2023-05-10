#ifndef MESHCRAFT_WIREFRAME_DEPTHNORMALS_CGINC
#define MESHCRAFT_WIREFRAME_DEPTHNORMALS_CGINC

#include  "UnityCG.cginc"
#include "../cginc/Wireframe_Core.cginc"



fixed4 _Color;
float _MC_VertexColor;
#ifdef MC_HAS_TEXTURE
	sampler2D _MainTex;
	half4 _MainTex_ST;
	half2 _MC_MainTex_Scroll;
#endif


struct v2f_surf
{
    float4 pos :SV_POSITION;
    half4 uv : TEXCOORD0; //xy - mainTex, zw - wireTex
    float3 worldPos : TEXCOORD1;
    fixed3 custompack2 : TEXCOORD2; //mass
    fixed4 color : TEXCOORD3;
    float4 nz : TEXCOORD4;
};


v2f_surf vert_surf(appdata_full v)
{
    v2f_surf o = (v2f_surf)0;


    o.pos = UnityObjectToClipPos(v.vertex);

    #ifdef MC_HAS_TEXTURE
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);		
		o.uv.xy += _MC_MainTex_Scroll.xy * _Time.x;
    #endif

    o.uv.zw = TRANSFORM_TEX(((_MC_WireTex_UVSet == 0) ? v.texcoord.xy : v.texcoord1.xy), _MC_WireTex);
    o.uv.zw += _MC_WireTex_Scroll.xy * _Time.x;
	
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.custompack2 = get_wireframe_info_from_vertex_uv(v.texcoord);
    o.color = v.color;

    //DepthNormals
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;

    return o;
}


//Pixel Shader///////////////////////////////////////////
fixed4 frag(v2f_surf i) : SV_Target
{
    //Color
    #if defined(MC_NO_COLOR_BLACK)
		fixed4 ret_color = 0;
    #elif defined(MC_NO_COLOR_WHITE)
		fixed4 ret_color = 1;
    #else
		fixed4 ret_color = _Color;
    #endif

    //Main Texture
    #ifdef MC_HAS_TEXTURE
		half4 mainTex = tex2D(_MainTex, i.uv.xy);
	    #if defined(MC_HAS_TEXTURE) && !defined(MC_NO_COLOR_BLACK) && !defined(MC_NO_COLOR_WHITE)
			ret_color *= mainTex;
	    #endif
    #endif

    //Vertex Color
    ret_color.rgb *= lerp(FIXED_3_ONE, i.color.rgb, _MC_VertexColor);
	
    const float fixed_size = distance(_WorldSpaceCameraPos, i.worldPos);
	
    //Wire
    #ifndef MC_NO
	    fixed4 wire_tex_color = tex2D(_MC_WireTex, i.uv.zw);
	    wire_tex_color.rgb *= lerp(FIXED_3_ONE, i.color.rgb, _MC_WireVertexColor);

	    float3 wire_emission = 0;

	    #ifdef MC_TRANSPARENCY_ON
			half customAlpha = wireTexColor.a;
			customAlpha = lerp(customAlpha, 1 - customAlpha, _MC_TransparentTex_Invert);
			customAlpha = saturate(customAlpha + _MC_TransparentTex_Alpha_Offset);
			_MC_Color.a *= customAlpha;
	    #endif

	    #ifdef MC_MULTIPLY
			ret_color.rgb = lerp(FIXED_3_ONE, ret_color.rgb, ret_color.a);
	    #endif

	    #ifdef MC_ADDITIVE
			ret_color.rgb = lerp(FIXED_3_ZERO, ret_color.rgb, ret_color.a);
	    #endif

	    DoWire(wire_tex_color, ret_color, i.custompack2, fixed_size, wire_emission);
    #endif


    return EncodeDepthNormal(i.nz.w, i.nz.xyz);
}


#endif
