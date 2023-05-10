// Preprocessor directives to avoid multiple inclusions of the header file.
#ifndef MESHCRAFT_WIREFRAME_COLORMASK0_CGINC
#define MESHCRAFT_WIREFRAME_COLORMASK0_CGINC

// Define a custom vertex structure named v2f with shader semantics.
struct v2f
{
    float4 pos : SV_POSITION; // Store clip space position of the vertex.

    UNITY_VERTEX_INPUT_INSTANCE_ID // Adds instance ID support to the shader.
    UNITY_VERTEX_OUTPUT_STEREO // Adds stereo rendering support to the shader.
};

// Vertex shader function that transforms vertex data from object space to clip space.
v2f vert(appdata_full v)
{
    v2f o; // Output vertex structure.
    UNITY_INITIALIZE_OUTPUT(v2f, o); // Initialize the output structure.

    UNITY_SETUP_INSTANCE_ID(v); // Set up the instance ID for this vertex.
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // Initialize stereo rendering.

    o.pos = UnityObjectToClipPos(v.vertex); // Transform vertex position from object space to clip space.

    return o; // Return the output vertex structure.
}

// Fragment shader function that returns a fixed color.
fixed4 frag() : SV_Target
{
    return 0; // Return black color with zero alpha.
}

// End of the preprocessor directive to avoid multiple inclusions of the header file.
#endif
