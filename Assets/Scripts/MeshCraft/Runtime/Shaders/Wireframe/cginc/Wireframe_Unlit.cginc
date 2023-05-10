#ifndef MESHCRAFT_WIREFRAME_UNLIT_CGINC
#define MESHCRAFT_WIREFRAME_UNLIT_CGINC

#include  "UnityCG.cginc"
#include "./Wireframe_Core.cginc"


#if defined(MC_REFLECTION_CUBE_SIMPLE) || defined(MC_REFLECTION_CUBE_ADVANED) || defined(MC_REFLECTION_UNITY_REFLECTION_PROBES)
	#define MC_REFLECTION_ON
#endif

//Variables//////////////////////////////////
fixed4 _Color;
float _MC_VertexColor;
#ifdef MC_HAS_TEXTURE
sampler2D _MainTex;
half4 _MainTex_ST;
half2 _MC_MainTex_Scroll;
#endif

#ifdef MC_REFLECTION_ON

	fixed4 _ReflectColor;
	half _MC_Reflection_Strength;
	half _MC_Reflection_Fresnel_Bias;

#if defined(MC_REFLECTION_CUBE_SIMPLE) || defined(MC_REFLECTION_CUBE_ADVANED)
		UNITY_DECLARE_TEXCUBE(_Cube);
#endif

#if defined(MC_REFLECTION_CUBE_ADVANED) || defined(MC_REFLECTION_UNITY_REFLECTION_PROBES)
		half _MC_Reflection_Roughness;
#endif
#endif


//Struct/////////////////////////////////////////////////////////
struct vInput
{
    float4 vertex : POSITION;

    half4 texcoord : TEXCOORD0;
    half4 texcoord1 : TEXCOORD1;

    #if defined(MC_REFLECTION_ON)
		half3 normal : NORMAL;
    #endif

    fixed4 color : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct vOutput
{
    float4 pos :SV_POSITION;
    half4 uv : TEXCOORD0; //xy - mainTex, zw - wireTex

    #ifdef MC_REFLECTION_ON
		half4 refl : TEXCOORD3; //xyz - reflection, w - fresnel	
    #endif

    fixed4 vColor : TEXCOORD4;


    fixed3 mass : TEXCOORD5;

    float2 data : TEXCOORD6; //x - fadeCoord, y - fixedSizeCoord

    UNITY_FOG_COORDS(8)

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


// Vertex Shader
vOutput vert(vInput v)
{
    vOutput o = (vOutput)0;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


    o.pos = UnityObjectToClipPos(v.vertex);

    #ifdef MC_HAS_TEXTURE
    o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
    o.uv.xy += _MC_MainTex_Scroll.xy * _Time.x;
    #endif

    o.uv.zw = TRANSFORM_TEX(((_MC_WireTex_UVSet == 0) ? v.texcoord.xy : v.texcoord1.xy), _MC_WireTex);
    o.uv.zw += _MC_WireTex_Scroll.xy * _Time.x;


    half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

    #if  defined(MC_REFLECTION_ON)
		half3 worldNormal = UnityObjectToWorldNormal(v.normal);
    #endif

    #if defined(MC_REFLECTION_ON)
		half fresnel = dot (normalize(ObjSpaceViewDir(v.vertex).xyz), v.normal);
    #endif



    #ifdef MC_REFLECTION_ON
		half3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
		o.refl.xyz = reflect( -worldViewDir, worldNormal );		
		
		o.refl.w = 1 - saturate(fresnel + _MC_Reflection_Fresnel_Bias);
		o.refl.w *= o.refl.w;
		o.refl.w *= o.refl.w;
    #endif


    o.vColor = v.color;

    //Fog
    UNITY_TRANSFER_FOG(o, o.pos);

    #ifndef MC_NO
    o.mass = get_wireframe_info_from_vertex_uv(v.texcoord);

    o.data.y = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
    #endif

    return o;
}


// Pixel Shader
fixed4 frag(vOutput i) : SV_Target
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
    retColor.rgb *= lerp(FIXED_3_ONE, i.vColor.rgb, _MC_VertexColor);


    //Reflection
    #ifdef MC_REFLECTION_ON
    #if defined(MC_REFLECTION_CUBE_SIMPLE)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE( _Cube, i.refl.xyz ) * _ReflectColor;
    #elif defined(MC_REFLECTION_CUBE_ADVANED)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE_LOD( _Cube, i.refl.xyz, _MC_Reflection_Roughness * 10) * _ReflectColor;
    #elif defined(MC_REFLECTION_UNITY_REFLECTION_PROBES)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, i.refl.xyz, _MC_Reflection_Roughness * 10) * _ReflectColor;
    #else
			fixed4 reflTex = _ReflectColor;
    #endif


    #ifdef MC_HAS_TEXTURE
			retColor.rgb = lerp(retColor.rgb, reflTex.rgb, saturate(mainTex.a + _MC_Reflection_Strength * 2 - 1) *  i.refl.w);
    #else
			retColor.rgb = lerp(retColor.rgb, reflTex.rgb, _MC_Reflection_Strength *  i.refl.w);
    #endif
    #endif


    //Wire
    #ifndef MC_NO

    fixed4 wireTexColor = tex2D(_MC_WireTex, i.uv.zw);
    wireTexColor.rgb *= lerp(FIXED_3_ONE, i.vColor.rgb, _MC_WireVertexColor);

    float3 wireEmission = 0;


    #ifdef MC_TRANSPARENCY_ON
				half customAlpha = wireTexColor.a;

				customAlpha = lerp(customAlpha, 1 - customAlpha, _MC_TransparentTex_Invert);
				 
				customAlpha = saturate(customAlpha + _MC_TransparentTex_Alpha_Offset);


				_MC_Color.a *= customAlpha;
    #endif

    #ifdef MC_MULTIPLY
				retColor.rgb = lerp(FIXED_3_ONE, retColor.rgb, retColor.a);
    #endif

    #ifdef MC_ADDITIVE
    retColor.rgb = lerp(FIXED_3_ZERO, retColor.rgb, retColor.a);
    #endif

    DoWire(wireTexColor, retColor, i.mass, i.data.y, wireEmission);


    //Emission
    retColor.rgb += wireEmission;

    #endif


    //Fog
    #if defined(MC_ADDITIVE)
    UNITY_APPLY_FOG_COLOR(i.fogCoord, retColor, fixed4(0,0,0,0)); // fog towards black due to our blend mode
    #elif defined(MC_MULTIPLY)
		UNITY_APPLY_FOG_COLOR(i.fogCoord, retColor, fixed4(1,1,1,1)); // fog towards white due to our blend mode
    #else
		UNITY_APPLY_FOG(i.fogCoord, retColor);
    #endif


    return retColor;
}


#endif
