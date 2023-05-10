#ifndef MESHCRAFT_WIREFRAME_CORE_CGINC
#define MESHCRAFT_WIREFRAME_CORE_CGINC


static const fixed3 FIXED_3_ONE = fixed3(1, 1, 1);
static const fixed3 FIXED_3_ZERO = fixed3(0, 0, 0);


half4 _MC_Color;
half _MC_EmissionStrength;
half _MC_LineWidth;


sampler2D _MC_WireTex;
half4 _MC_WireTex_ST;
half _MC_WireTex_UVSet;
half2 _MC_WireTex_Scroll;
float _MC_WireVertexColor;
#ifdef MC_TRANSPARENCY_ON
	half _MC_TransparentTex_Invert;
	half _MC_TransparentTex_Alpha_Offset;
#endif

float3 _MC_ObjectWorldPos;


#ifdef UNITY_PASS_META
	#ifdef MC_TRANSPARENT
		#define EMISSION_STRENGTH 0 
	#else
		#define EMISSION_STRENGTH 1;
	#endif
#else
	#define EMISSION_STRENGTH _MC_EmissionStrength;
#endif

// Extracts wireframe data from vertex UV.
inline float3 get_wireframe_info_from_vertex_uv(half4 texcoord)
{
    return float3(floor(texcoord.z), frac(texcoord.z) * 10, texcoord.w);
}

// Extracts wireframe data from mass with distance fade.
inline half get_wireframe_info_from_mass(half3 mass, float fixedSize)
{
    float size = _MC_LineWidth;
    #ifndef MC_ANTIALIASING_DISABLE
    half3 width = abs(ddx(mass)) + abs(ddy(mass));
    half3 eF = smoothstep(0, width * size, mass);

    return size > 0 ? min(min(eF.x, eF.y), eF.z) : 1;
    #else
			return step(size, min(min(mass.x, mass.y), mass.z));
    #endif
}


// Wireframe rendering function in opaque mode.
inline half WireOpaque(fixed4 wireTexColor, inout fixed4 srcColor, fixed3 mass, float fixedSize, out float3 emission)
{
	const half value = get_wireframe_info_from_mass(mass, fixedSize);
	const float wire_color_alpha = _MC_Color.a;
    srcColor = lerp(lerp(srcColor, _MC_Color * wireTexColor, wire_color_alpha), srcColor, value);
    emission = wireTexColor.rgb * _MC_Color.rgb * (1 - value) * wire_color_alpha * EMISSION_STRENGTH
    return value;
}

#ifdef MC_TRANSPARENT
	// Wireframe rendering function in transparent mode.
	inline half WireTransparent(fixed4 wireTexColor, inout fixed4 srcColor, fixed3 mass, float fixedSize, out float3 emission)
	{
		half value = get_wireframe_info_from_mass(mass, fixedSize);
		float4 wColor = _MC_Color * wireTexColor;
		
	#ifdef MC_SAME_COLOR
			srcColor.rgb = wColor.rgb;
	#endif
		
		float wireColorAlpha = _MC_Color.a;
		
		half3 wireColor = lerp(lerp(wColor.rgb, srcColor.rgb, srcColor.a), wColor.rgb, wireColorAlpha);
		wireColor = lerp(srcColor.rgb, wireColor, wireColorAlpha);

		srcColor.rgb = lerp(wireColor.rgb, srcColor.rgb, value);
		srcColor.a = saturate(srcColor.a + (1 - value) * wireColorAlpha);
		
		emission = wColor.rgb * (1 - value) * wireColorAlpha * EMISSION_STRENGTH;
		return value;
	}
#endif


#if defined(MC_TRANSPARENT)
	#define DoWire(wireTexColor,srcColor,mass,fixedsize,emission) WireTransparent(wireTexColor,srcColor,mass,fixedsize,emission)
#else
	#define DoWire(wireTexColor,srcColor,mass,fixedsize,emission) WireOpaque(wireTexColor,srcColor,mass,fixedsize,emission)
#endif

#endif
