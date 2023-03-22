Shader "LiteNinja/BloomVFX"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #pragma multi_compile_local _ _USE_RGBM
    UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
    UNITY_DECLARE_SCREENSPACE_TEXTURE(bloom_tex);
    half4 _MainTex_TexelSize;
    half4 _MainTex_ST;
    half blur_amount;
    half4 bloom_color;
    half4 bloom_data;
    half _BloomAmount;
    int _DownSampleFactor;

    struct appdata
    {
        half4 pos : POSITION;
        half2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        half4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    struct v2fb
    {
        half4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        half4 uv1 : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    // Unpacks the color value from the given packed color format.
    //
    // This function takes a packed color value (c) as input and unpacks
    // it according to the color space and encoding used in the shader.
    // It returns the unpacked color value as a float3.
    //
    // Parameters:
    // float4 c: The packed color value to be unpacked.
    //
    // Returns:
    // float3: The unpacked color value in linear color space.
    float3 unpack(float4 c)
    {
        // If the shader is in gamma color space, square the RGB values
        // to convert them to linear color space.
        #if UNITY_COLORSPACE_GAMMA
        c.rgb *= c.rgb;
        #endif

        // If the shader is using RGBM encoding, unpack the color by
        // multiplying the RGB values by the M value (alpha) and a
        // scaling factor of 8.0. Otherwise, simply return the RGB values.
        #if _USE_RGBM
        return c.xyz * c.w * 8.0;
        #else
        return c.rgb;
        #endif
    }


    // Packs the color value into a specified format for storage and usage.
    //
    // This function takes an unpacked color value (c) as input and packs
    // it according to the color space and encoding used in the shader.
    // It returns the packed color value as a half4.
    //
    // Parameters:
    // half3 c: The unpacked color value to be packed.
    //
    // Returns:
    // half4: The packed color value in the specified color space and encoding.
    half4 pack(half3 c)
    {
        // If the shader is using RGBM encoding, pack the color by
        // scaling the RGB values by 0.125 and calculating the M value (alpha)
        // based on the maximum of the scaled RGB values. Otherwise, simply
        // set the alpha value to 1.0.
        #if _USE_RGBM
        c *= 0.125;
        half m = max(max(c.x, c.y), max(c.z, 1e-5));
        m = ceil(m * 255) / 255;
        half4 o = half4(c / m, m);
        #else
        half4 o = half4(c, 1.0);
        #endif

        // If the shader is in gamma color space, square root the RGB values
        // to convert them to gamma color space. Otherwise, simply return the
        // packed color value.
        #if UNITY_COLORSPACE_GAMMA
    return half4(sqrt(o.rgb), o.a);
        #else
        return o;
        #endif
    }


    // Calculates the vertex data for the bloom blur pass.
    //
    // This function takes the input appdata (i) and computes the vertex
    // data required for the bloom blur pass. It sets up stereo rendering
    // and calculates the texture coordinates with adjusted offsets for
    // the bloom blur effect.
    //
    // Parameters:
    // appdata i: The input vertex data.
    //
    // Returns:
    // v2fb: The calculated vertex data for the bloom blur pass.
    v2fb calc_blur_vertex_data(const appdata i)
    {
        v2fb o;
        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_INITIALIZE_OUTPUT(v2fb, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(i.pos);
        o.uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);

        // Calculate the offset in texture coordinates for the bloom blur effect.
        const half2 offset = _MainTex_TexelSize * (1.0 / _MainTex_ST.xy);

        // Store the adjusted texture coordinates with the calculated offset.
        o.uv1 = half4(
            UnityStereoScreenSpaceUVAdjust(i.uv - offset, _MainTex_ST),
            UnityStereoScreenSpaceUVAdjust(i.uv + offset, _MainTex_ST));

        return o;
    }


    // Calculates the vertex data for the bloom pass.
    //
    // This function takes the input appdata (i) and computes the vertex
    // data required for the bloom pass. It sets up stereo rendering
    // and calculates the texture coordinates for the bloom effect.
    //
    // Parameters:
    // appdata i: The input vertex data.
    //
    // Returns:
    // v2f: The calculated vertex data for the bloom pass.
    v2f calc_vertex_data(const appdata i)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(i.pos);

        // Calculate the texture coordinates for the bloom pass.
        o.uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);

        return o;
    }


    // Calculates the bloom intensity of the fragment.
    //
    // This function takes the input vertex data (i) and computes the bloom
    // intensity for each fragment. It accounts for gamma correction and
    // uses bloom_data to adjust the bloom intensity.
    //
    // Parameters:
    // v2f i: The input vertex data.
    //
    // Returns:
    // half4: The packed color data with bloom intensity applied.
    half4 calc_intensity(const v2f i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        // Sample the screenspace texture and apply gamma correction if needed.
        half3 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv).xyz;
        #if UNITY_COLORSPACE_GAMMA
        c.rgb *= c.rgb;
        #endif

        // Calculate the brightness and soft threshold for bloom.
        const half br = max(c.r, max(c.g, c.b));
        const half soft = clamp(br - bloom_data.y, 0.0, bloom_data.z);

        // Check if the brightness is above the threshold.
        if (br > bloom_data.x)
        {
            // Compute the bloom intensity based on the brightness and soft threshold.
            const half a = max(soft * soft * bloom_data.w, br - bloom_data.x) / max(br, 1e-4);

            // Return the packed color data with the calculated bloom intensity applied.
            return pack(c * a);
        }
        else
        {
            // Return the packed color data without bloom intensity applied.
            return pack(c);
        }
    }



    // Applies a Gaussian blur to the fragment.
    //
    // This function takes the input vertex data (i) and performs a Gaussian blur
    // operation on the fragment by sampling the screenspace texture at different
    // offsets. The result is a blurred fragment color.
    //
    // Parameters:
    // v2fb i: The input vertex data.
    //
    // Returns:
    // half4: The packed color data with Gaussian blur applied.
    half4 apply_gaussian_blur(v2fb i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        // Sample the screenspace texture at the current UV coordinate and
        // unpack the color data.
        half3 c = unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv));

        // Add the color data from neighboring texels to create a Gaussian blur effect.
        c += unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xy));
        c += unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.xw));
        c += unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zy));
        c += unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1.zw));

        // Return the packed color data with the blur applied, using a weight of 0.2.
        return pack(c * 0.2);
    }
    

    // Combines the main texture and bloom texture using linear interpolation.
    //
    // This function takes the input vertex data (i) and combines the main texture
    // with the bloom texture using linear interpolation based on the blur_amount.
    //
    // Parameters:
    // v2f i: The input vertex data.
    //
    // Returns:
    // float4: The packed color data after linear interpolation between the main
    //         and bloom textures.
    float4 blend_bloom_tex_with_main_tex(const v2f i) : SV_Target
    {
        // Set up stereo eye index for VR rendering if necessary
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        // Sample and unpack the color data from the main texture and bloom texture
        // and perform a linear interpolation between the main texture color and
        // bloom texture color based on the blur_amount.
        return pack(lerp(unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv)),
                         unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(bloom_tex, i.uv)), blur_amount));
    }


    // Combines the main texture and the bloom texture, applying bloom color and adjusting for color space.
    //
    // This function takes the input vertex data (i), samples the main texture and bloom texture,
    // applies the bloom color, and adjusts for the color space (gamma or linear) before returning
    // the final color.
    //
    // Parameters:
    // v2f i: The input vertex data.
    //
    // Returns:
    // half4: The final color data after combining the main texture, bloom texture, and applying bloom color.
    half4 apply_bloom_with_color_correction(const v2f i) : SV_Target
    {
        // Set up stereo eye index for VR rendering if necessary
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        // Sample the main texture
        half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);

        // Adjust for gamma color space if necessary
        #if UNITY_COLORSPACE_GAMMA
        c.rgb *= c.rgb;
        #endif

        // Sample the bloom texture and apply the bloom color
        const half3 bloom = unpack(UNITY_SAMPLE_SCREENSPACE_TEXTURE(bloom_tex, i.uv));
        c.rgb += bloom * bloom_color.rgb;

        // Return the final color, adjusting for gamma color space if necessary
        #if UNITY_COLORSPACE_GAMMA
        return half4(sqrt(c.rgb), c.a);
        #else
        return c;
        #endif
    }
    ENDCG

    Subshader
    {
        LOD 100
        ZTest Always ZWrite Off Cull Off
        Fog
        {
            Mode off
        }

        // Pass 0: Extract the bright areas from the screen
        Pass //0
        {
            CGPROGRAM
            #pragma vertex calc_blur_vertex_data
            #pragma fragment calc_intensity
            ENDCG
        }
        // Pass 1: Apply a Gaussian blur to the extracted bright areas (horizontal direction)
        Pass //1
        {
            CGPROGRAM
            #pragma vertex calc_blur_vertex_data
            #pragma fragment apply_gaussian_blur
            ENDCG
        }
        // Pass 2: Blend the blurred bright areas back into the original image
        Pass //2
        {
            CGPROGRAM
            #pragma vertex calc_vertex_data
            #pragma fragment blend_bloom_tex_with_main_tex
            ENDCG
        }
        // Pass 3: Apply the final bloom effect by adding the blurred bright areas to the original image
        Pass //3
        {
            CGPROGRAM
            #pragma vertex calc_vertex_data
            #pragma fragment apply_bloom_with_color_correction
            ENDCG
        }
    }
    Fallback off
}