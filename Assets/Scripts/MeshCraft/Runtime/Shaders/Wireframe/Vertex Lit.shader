Shader "LiteNinja/MeshCraft/Wireframe/Vertex Lit" 
{ 
	Properties 
	{   
		
		//Visual Options
		[MC_Label] _MC_Label_V_Options("Default Visual Options", float) = 0  
		
		//Base 
		_Color("Color (RGB)", color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white"{}			
		[MC_UVScroll] _MC_MainTex_Scroll("    ", vector) = (0, 0, 0, 0)
		
		//Vertex Color
		[MC_Toggle] _MC_VertexColor ("Vertex Color", float) = 0	
		 
		 

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

		//Light
		[MC_IncludeLight] _MC_IncludeLightEnumID ("", float) = 0

		//Transparency          
		[MC_Label]		  _MC_Transparency_M_Options("Wire Transparency Options", float) = 0  
		[MC_Transparency] _MC_TransparencyEnumID("", float) = 0 				
		[HideInInspector]	  _MC_TransparentTex_Invert("    ", float) = 0
		[HideInInspector]	  _MC_TransparentTex_Alpha_Offset("    ", Range(-1, 1)) = 0
		
	} 
	    
	Category      
	{
		Tags { "RenderType"="Wireframe_Opaque" }   
		LOD 150 
	 
		SubShader  
		{			  
		 
			// Vertex Lit, emulated in shaders (4 lights max, no specular)
			Pass  
			{
				Tags { "LightMode" = "Vertex" }
				Lighting On 
				 
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders


				#pragma multi_compile_fog


				
				
				#pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
				#pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


				#define MC_HAS_TEXTURE

				#include "cginc/Wireframe_VertexLit.cginc"
				
				ENDCG
			}
		 
			// Lightmapped
			Pass 
			{
				Tags { "LightMode" = "VertexLM" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders

				#pragma multi_compile_fog


				

				#pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
				#pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


				#define MC_LIGHTMAP_ON
				#define MC_HAS_TEXTURE

				#include "cginc/Wireframe_VertexLit.cginc"
				 
				ENDCG         
			}    
		     
			// Lightmapped, encoded as RGBM
			Pass 
	 		{
				Tags { "LightMode" = "VertexLMRGBM" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag 
				#pragma target 3.0
				#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders

				#pragma multi_compile_fog 


				

				#pragma shader_feature MC_LIGHT_OFF MC_LIGHT_ON
				#pragma shader_feature MC_TRANSPARENCY_OFF MC_TRANSPARENCY_ON


				#define MC_LIGHTMAP_ON
				#define MC_HAS_TEXTURE

				#include "cginc/Wireframe_VertexLit.cginc"
				 
				ENDCG
			}
			 
			// Pass to render object as a shadow caster
			Pass 
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
		
				CGPROGRAM
				#pragma vertex vert_surf   
				#pragma fragment frag 
				#pragma multi_compile_shadowcaster 
				#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
				#include "UnityCG.cginc"  

				
				

				#define MC_NO_COLOR_BLACK

				#include "cginc/Wireframe_Shadow.cginc" 			
				ENDCG 
			}
		}
	}

	FallBack Off
}
 
