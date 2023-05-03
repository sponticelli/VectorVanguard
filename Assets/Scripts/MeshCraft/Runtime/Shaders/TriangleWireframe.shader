Shader "LiteNinja/MeshCraft/TriangleWireframe"
{
    Properties
    {
        // Wire color with HDR support
        [HDR] _WireColor ("Wire color", Color) = (0, 0, 0, 1)
        // Wire thickness
        _WireSize ("Wire size", float) = 0.3
        // Offset along z-axis to prevent z-fighting
        _ZMode("Z Mode", Range(-1,1)) = 1
        // Determines the faces to render
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }

    SubShader
    {
        Tags
        {
            // Renders the object as transparent and places it in the transparent render queue
            "RenderType"="Transparent" "Queue"="Transparent"
        }

        Pass
        {

            // Enables alpha blending
            Blend SrcAlpha OneMinusSrcAlpha
            // Use the _Cull property to determine which faces to render
            Cull[_Cull]
            // Do not write to the depth buffer
            ZWrite Off

            CGPROGRAM
            #include "UnityCG.cginc"
            // Declare properties
            float _ZMode;
            float _WireSize;
            fixed4 _WireColor;

            // Input structure for the vertex shader
            struct appdata
            {
                // Vertex position, normal, and texture coordinates
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Output structure for the vertex shader
            struct v2f
            {
                // Transformed vertex position and texture coordinates
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Vertex shader
            v2f vert(const appdata v)
            {
                v2f m;
                // Transform the vertex position to clip space
                m.vertex = UnityObjectToClipPos(v.vertex);
                // Offset the vertex along the z-axis to prevent z-fighting
                m.vertex.z -= (0.001 * _ZMode);
                // Pass through the texture coordinates
                m.uv = v.uv;
                return m;
            }

            // Utility function to calculate the squared distance between two points
            float squared_distance_between_points(const float2 point_1, const float2 point_2)
            {
                const float2 vector_between_points = point_2 - point_1;
                return dot(vector_between_points, vector_between_points);
            }

            // Utility function to calculate the minimum distance from a point to a line segment
            float minimum_distance_from_point_to_line_segment(const float2 point_a, const float2 point_b,
                                                              const float2 test_point)
            {
                const float segment_squared_length = squared_distance_between_points(point_a, point_b);
                const float projection_scalar = max(
                    0, min(1, dot(test_point - point_a, point_b - point_a) / segment_squared_length));
                const float2 projection_point = point_a + projection_scalar * (point_b - point_a);
                return distance(test_point, projection_point);
            }


            // Fragment shader
            fixed4 frag(v2f input_data) : SV_Target
            {
                // Setup and compute distances in UV space to generate the wireframe effect
                const float wireframe_size = _WireSize;
                const float line_antialiasing = 1;

                // Compute the rate of change in the UV coordinates in screen space
                const float2 u_change_rate = float2(ddx(input_data.uv.x), ddy(input_data.uv.x));
                const float2 v_change_rate = float2(ddx(input_data.uv.y), ddy(input_data.uv.y));

                // Compute the length of the rate of change vectors
                const float u_change_length = length(u_change_rate);
                const float v_change_length = length(v_change_rate);
                const float uv_change_length = length(u_change_rate + v_change_rate);

                // Compute the minimum distance to the U and V edges of the triangle in UV space, scaled by the wireframe size
                const float min_distance_to_u = wireframe_size * v_change_length;
                const float min_distance_to_v = wireframe_size * u_change_length;
                const float min_distance_to_uv = wireframe_size * uv_change_length;

                // Compute the distance of the fragment to the U and V edges of the triangle in UV space
                const float u_edge_distance = input_data.uv.x;
                const float v_edge_distance = input_data.uv.y;
                const float opposite_u_edge_distance = (1.0 - u_edge_distance);
                const float opposite_v_edge_distance = (1.0 - v_edge_distance);

                // Find the smallest distance to the U and V edges
                const float min_u_edge_distance = min(u_edge_distance, opposite_u_edge_distance);
                const float min_v_edge_distance = min(v_edge_distance, opposite_v_edge_distance);

                // Calculate the minimum distance to the diagonal in UV space
                const float uv_diagonal_distance = minimum_distance_from_point_to_line_segment(
                    float2(0.0, 1.0), float2(1.0, 0.0), input_data.uv);

                // Normalize the distances by dividing by the minimum distances
                const float normalized_u_distance = min_u_edge_distance / min_distance_to_u;
                const float normalized_v_distance = min_v_edge_distance / min_distance_to_v;
                const float normalized_uv_diagonal_distance = uv_diagonal_distance / min_distance_to_uv;

                // Find the smallest of the normalized distances
                float smallest_normalized_distance = min(normalized_u_distance, normalized_v_distance);
                smallest_normalized_distance = min(smallest_normalized_distance, normalized_uv_diagonal_distance);

                // Compute the alpha value for the wireframe lines
                float line_alpha = 1.0 - smoothstep(1.0, 1.0 + (line_antialiasing / wireframe_size),
                                                    smallest_normalized_distance);
                // Multiply by the alpha value of the wire color
                line_alpha *= _WireColor.a;
                
                // Return the wire color with the computed alpha value
                return fixed4(_WireColor.rgb, line_alpha);
            }

            // Specify the functions to use as the vertex and fragment shaders
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}