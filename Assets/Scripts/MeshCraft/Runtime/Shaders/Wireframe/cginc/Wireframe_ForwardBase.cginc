#ifndef MESHCRAFT_WIREFRAME_FORWARDBASE_CGINC
#define MESHCRAFT_WIREFRAME_FORWARDBASE_CGINC

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "Wireframe_Core.cginc"


//not enough registers
#ifdef UNITY_VERTEX_OUTPUT_STEREO
#ifdef MC_SPECULAR
#undef MC_SPECULAR
#endif

#ifdef _NORMALMAP
	#undef _NORMALMAP
#endif
#endif

#ifdef _NORMALMAP
	#define MC_LIGHTDIR i.lightDir
#else
#define MC_LIGHTDIR _WorldSpaceLightPos0.xyz
#endif

#if defined(MC_REFLECTION_CUBE_SIMPLE) || defined(MC_REFLECTION_CUBE_ADVANED) || defined(MC_REFLECTION_UNITY_REFLECTION_PROBES)
	#define MC_REFLECTION_ON
#endif

//not enough registers
#if defined(MC_REFLECTION_ON) && defined(_NORMALMAP)
#ifdef MC_SPECULAR
	#undef MC_SPECULAR
#endif
#endif


//Variables//////////////////////////////////
fixed4 _Color;
float _MC_VertexColor;
#ifdef MC_HAS_TEXTURE
sampler2D _MainTex;
half4 _MainTex_ST;
half2 _MC_MainTex_Scroll;
#endif

#ifdef _NORMALMAP
	sampler2D _MC_NormalMap;
#endif

#ifdef MC_SPECULAR
	sampler2D _MC_Specular_Lookup;
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

    half3 normal : NORMAL;
    half4 tangent : TANGENT;

    fixed4 color : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct vOutput
{
    float4 pos :SV_POSITION;

    half4 uv : TEXCOORD0; //xy - mainTex, zw - wireTex


    half4 worldPos : TEXCOORD1; //xyz - worldPos, w - fixedSizeCoord


    half4 normal : TEXCOORD2; //xyz - normal, w - fresnel


    #ifdef MC_REFLECTION_ON
		half4 refl : TEXCOORD3; //xyz - reflection, w - fresnel	
    #endif

    fixed4 vColor : TEXCOORD4;
    float4 mass : TEXCOORD5; //xyz - mass, w - fadeCoord
    UNITY_FOG_COORDS(6)


    #ifndef LIGHTMAP_OFF
		UNITY_VERTEX_OUTPUT_STEREO	//TEXCOORD7 used by Unity!
		half2 lm : TEXCOORD8;
    #else

    #if defined(MC_SPECULAR ) || defined(_NORMALMAP)

    #ifdef MC_SPECULAR
				half4 viewDir : TEXCOORD7;	//xyz - viewdir, w - specular(nh)
    #endif

    #ifdef _NORMALMAP
				half3 lightDir : TEXCOORD8;
    #endif
    #else
		UNITY_VERTEX_OUTPUT_STEREO //TEXCOORD7 used by Unity!
    #endif

    half4 vLight : TEXCOORD9;
    UNITY_SHADOW_COORDS(10)
    #endif
};

//Vertex Shader///////////////////////////////////////////
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


    const half3 world_pos = mul(unity_ObjectToWorld, half4(v.vertex.xyz, 1)).xyz;
    float3 normal_ws = UnityObjectToWorldNormal(v.normal);

    #if defined(MC_REFLECTION_ON) 
		half fresnel = dot (normalize(ObjSpaceViewDir(v.vertex).xyz), v.normal);
    #endif


    o.worldPos.xyz = world_pos;
	


    #ifdef MC_REFLECTION_ON
		half3 worldViewDir = UnityWorldSpaceViewDir(worldPos);
		o.refl.xyz = reflect( -worldViewDir, normal_WS );		
		o.refl.w = 1 - saturate(fresnel + _MC_Reflection_Fresnel_Bias);
		o.refl.w *= o.refl.w;
		o.refl.w *= o.refl.w;
    #endif


    o.vColor = v.color;
	
    //Fog
    UNITY_TRANSFER_FOG(o, o.pos);

    #ifndef MC_NO
	    o.mass.xyz = get_wireframe_info_from_vertex_uv(v.texcoord);
	    o.worldPos.w = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
    #endif


    #ifndef LIGHTMAP_OFF
		o.lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #else
	    #ifdef UNITY_SHOULD_SAMPLE_SH
		    o.vLight.rgb = ShadeSH9(half4(normal_ws, 1.0));

		    #ifdef VERTEXLIGHT_ON
				float3 pos_WS = mul(unity_ObjectToWorld, v.vertex).xyz
				o.vLight.rgb += Shade4PointLights ( unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, pos_WS, normal_WS );
		    #endif
	    #endif


	    #ifdef _NORMALMAP
			TANGENT_SPACE_ROTATION;
			o.lightDir = normalize(mul (rotation, ObjSpaceLightDir(v.vertex)));
		    #ifdef MC_SPECULAR
				o.viewDir.xyz = mul (rotation, normalize(ObjSpaceViewDir(v.vertex)));
		    #endif
	    #else
		    #ifdef MC_SPECULAR
				o.viewDir.xyz = WorldSpaceViewDir(v.vertex);
		    #endif
	    #endif
    #endif

    o.normal.xyz = normal_ws;

    #ifdef LIGHTMAP_OFF
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    #endif

    return o;
}

//Pixel Shader///////////////////////////////////////////
fixed4 frag(vOutput i) : SV_Target
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
    ret_color.rgb *= lerp(FIXED_3_ONE, i.vColor.rgb, _MC_VertexColor);


    #ifdef _NORMALMAP
		fixed3 bumpNormal = UnpackNormal(tex2D(_MC_NormalMap, i.uv.xy));
    #endif

    #ifndef LIGHTMAP_OFF
		fixed3 diff = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lm));
		retColor.rgb *= diff;
    #else
    UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos.xyz)

    #ifdef _NORMALMAP
			half3 normal = bumpNormal;				
    #else
    half3 normal = normalize(i.normal.xyz);
    #endif

    fixed3 diff = _LightColor0.rgb * atten;

    #ifdef MC_USE_LIGHT_RAMP_TEXTURE
			fixed2 rampUV = fixed2(max(0, dot(normal, MC_LIGHTDIR)), 0.5);
			diff *= tex2D(_MC_LightRampTex, rampUV);
    #else
    diff *= max(0, dot(normal, MC_LIGHTDIR));
    #endif

    diff += i.vLight.rgb;


    ret_color.rgb *= diff;


    #ifdef MC_SPECULAR
			half nh = max (0, dot (normal, normalize (MC_LIGHTDIR + normalize(i.viewDir.xyz))));
			fixed3 specular = tex2D(_MC_Specular_Lookup, half2(nh, 0.5)).rgb * retColor.a * _LightColor0.rgb * atten;

			retColor.rgb += specular;
    #endif
    #endif


    #ifdef MC_LIGHT_ON
		_MC_Color.rgb = diff * _MC_Color.rgb; 
    #endif


    //Reflection
    #ifdef MC_REFLECTION_ON

    #ifdef _NORMALMAP
			i.refl.xyz += bumpNormal * 0.25;
    #endif

    #if defined(MC_REFLECTION_CUBE_SIMPLE)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE( _Cube, i.refl.xyz ) * _ReflectColor;
    #elif defined(MC_REFLECTION_CUBE_ADVANED)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE_LOD( _Cube, i.refl.xyz, _MC_Reflection_Roughness * 10) * _ReflectColor;
    #elif defined(MC_REFLECTION_UNITY_REFLECTION_PROBES)
			fixed4 reflTex = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, i.refl.xyz, _MC_Reflection_Roughness * 10) * _ReflectColor;
			reflTex.xyz = DecodeHDR(reflTex, unity_SpecCube0_HDR);
    #else
			fixed4 reflTex = _ReflectColor;
    #endif


    #ifdef MC_HAS_TEXTURE
			retColor.rgb = lerp(retColor.rgb, retColor.rgb + reflTex.rgb, saturate(mainTex.a + _MC_Reflection_Strength * 2 - 1) *  i.refl.w);
    #else
			retColor.rgb = lerp(retColor.rgb, retColor.rgb + reflTex.rgb, _MC_Reflection_Strength *  i.refl.w);
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
	

    DoWire(wireTexColor, ret_color, i.mass.xyz,  i.worldPos.w, wireEmission);


    //Emission
    ret_color.rgb += wireEmission;

    #endif


    //Fog
    UNITY_APPLY_FOG(i.fogCoord, retColor);

    //Alpha
    #if  !defined(MC_TRANSPARENT)
    UNITY_OPAQUE_ALPHA(ret_color.a);
    #endif

    return ret_color;
}


#endif
