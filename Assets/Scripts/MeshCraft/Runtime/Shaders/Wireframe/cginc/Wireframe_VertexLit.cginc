#ifndef MESHCRAFT_WIREFRAME_VERTEXLIT_CGINC
#define MESHCRAFT_WIREFRAME_VERTEXLIT_CGINC

fixed4 _Color;
float _MC_VertexColor;
sampler2D _MainTex;
half4 _MainTex_ST;
half2 _MC_MainTex_Scroll;


#include "UnityCG.cginc"
#include "./Wireframe_Core.cginc"


struct v2f
{
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;

    #ifdef MC_LIGHTMAP_ON
    half2 lm : TEXCOORD1;
    #else
		half4 vLight : TEXCOORD1;
    #endif

    fixed4 color : TEXCOORD2;


    half3 mass : TEXCOORD4;

    float2 data : TEXCOORD6; //x - fadeCoord, y - fixedSizeCoord

    //FOG
    UNITY_FOG_COORDS(5)

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


v2f vert(appdata_full v)
{
    v2f o;
    UNITY_INITIALIZE_OUTPUT(v2f, o);

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    o.uv.xy += _MC_MainTex_Scroll.xy * _Time.x;

    o.uv.zw = TRANSFORM_TEX(((_MC_WireTex_UVSet == 0) ? v.texcoord.xy : v.texcoord1.xy), _MC_WireTex);
    o.uv.zw += _MC_WireTex_Scroll.xy * _Time.x;


    #ifdef MC_LIGHTMAP_ON
    o.lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #else
		float4 lighting = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1);
		o.vLight = lighting;
    #endif


    o.color = v.color;


    //FOG
    UNITY_TRANSFER_FOG(o, o.pos);

    #ifndef MC_NO
    o.mass = get_wireframe_info_from_vertex_uv(v.texcoord);
    o.data.y = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
    #endif

    return o;
}


fixed4 frag(v2f i) : SV_Target
{
    //Color
    #if defined(MC_NO_COLOR_BLACK)
		fixed4 retColor = 0;
    #elif defined(MC_NO_COLOR_WHITE)
		fixed4 retColor = 1;
    #else
    fixed4 retColor = _Color;
    #endif

    //Main Texture
    #ifdef MC_HAS_TEXTURE
    half4 mainTex = tex2D(_MainTex, i.uv.xy);

    #if defined(MC_HAS_TEXTURE) && !defined(MC_NO_COLOR_BLACK) && !defined(MC_NO_COLOR_WHITE)
    retColor *= mainTex;
    #endif
    #endif

    //Vertex Color
    retColor.rgb *= lerp(FIXED_3_ONE, i.color.rgb, _MC_VertexColor);


    #ifdef MC_LIGHTMAP_ON
    half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lm));

    retColor.rgb *= lm.rgb;

    #ifdef MC_LIGHT_ON
			_MC_Color.rgb *= lm.rgb;	
    #endif
    #else
		retColor *= i.vLight;

    #ifdef MC_LIGHT_ON
			_MC_Color.rgb *= i.vLight.rgb;
    #endif
    #endif


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

    DoWire(wire_tex_color, retColor, i.mass, i.data.y, wire_emission);


    //Emission
    retColor.rgb += wire_emission;


    #endif


    //Fog
    UNITY_APPLY_FOG(i.fogCoord, retColor);

    //Alpha
    #if  !defined(MC_TRANSPARENT)
    UNITY_OPAQUE_ALPHA(retColor.a);
    #endif

    return retColor;
}

#endif
