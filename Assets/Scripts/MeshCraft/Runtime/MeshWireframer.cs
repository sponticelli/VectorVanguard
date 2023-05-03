using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public class MeshWireframer
  {
    /// <summary>
    /// Creates a duplicate of the input mesh with modified UV coordinates for wireframe rendering.
    /// </summary>
    /// <param name="originalMesh">The original mesh to be duplicated and modified for wireframe rendering.</param>
    /// <returns>The duplicated and modified mesh suitable for wireframe rendering.
    /// If the vertex count of the original mesh is too high, returns null.</returns>
    public static Mesh CreateTriangleWireframeWithShader(Mesh mesh)
    {
      // Set the maximum number of vertices that Unity can handle.
      const int maxVertices = 65535;

      // Gather the necessary data from the original mesh.
      var normals = mesh.normals;
      var triangles = mesh.triangles;
      var vertices = mesh.vertices;
      var boneWeights = mesh.boneWeights;

      // Calculate the number of vertices needed for the new mesh.
      var requiredVertices = triangles.Length;

      // If the number of required vertices exceeds the maximum, log an error and return null.
      if (requiredVertices > maxVertices)
      {
        Debug.LogError($"Too many vertices! {requiredVertices} > {maxVertices}");
        return null;
      }

      // Create a new mesh to store the wireframe.
      var wireframeMesh = new Mesh
      {
        indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
      };

      // Create arrays to store the vertices, UVs, triangles, normals, and bone weights for the new mesh.
      var wireframeVertices = new Vector3[requiredVertices];
      var wireframeUVs = new Vector2[requiredVertices];
      var wireframeTriangles = new int[triangles.Length];
      var wireframeNormals = new Vector3[requiredVertices];
      var boneWeightsLength = (boneWeights.Length > 0) ? requiredVertices : 0;
      var wireframeBoneWeights = new BoneWeight[boneWeightsLength];

      // For each triangle in the original mesh, set the vertices, UVs, triangles, normals, and bone weights in the new mesh.
      for (var i = 0; i < triangles.Length; i += 3)
      {
        wireframeVertices[i] = vertices[triangles[i]];
        wireframeVertices[i + 1] = vertices[triangles[i + 1]];
        wireframeVertices[i + 2] = vertices[triangles[i + 2]];
        wireframeUVs[i] = new Vector2(0f, 0f);
        wireframeUVs[i + 1] = new Vector2(1f, 0f);
        wireframeUVs[i + 2] = new Vector2(0f, 1f);
        wireframeTriangles[i] = i;
        wireframeTriangles[i + 1] = i + 1;
        wireframeTriangles[i + 2] = i + 2;
        wireframeNormals[i] = normals[triangles[i]];
        wireframeNormals[i + 1] = normals[triangles[i + 1]];
        wireframeNormals[i + 2] = normals[triangles[i + 2]];

        if (wireframeBoneWeights.Length <= 0) continue;
        wireframeBoneWeights[i] = boneWeights[triangles[i]];
        wireframeBoneWeights[i + 1] = boneWeights[triangles[i + 1]];
        wireframeBoneWeights[i + 2] = boneWeights[triangles[i + 2]];
      }

      // Set the vertices, UVs, triangles, normals, bind poses, and bone weights of the new mesh.
      wireframeMesh.vertices = wireframeVertices;
      wireframeMesh.uv = wireframeUVs;
      wireframeMesh.triangles = wireframeTriangles;
      wireframeMesh.normals = wireframeNormals;
      wireframeMesh.bindposes = mesh.bindposes;
      wireframeMesh.boneWeights = wireframeBoneWeights;
      // Return the newly created wireframe mesh.
      return wireframeMesh;
    }

    public static Mesh CreateTriangleWireframeWithShader(GameObject targetObject)
    {
      var mesh = MeshHelper.GetMesh(targetObject);

      if (mesh != null) return CreateTriangleWireframeWithShader(mesh);

      Debug.LogError("No mesh found on target object!");
      return null;
    }

  
  }
}