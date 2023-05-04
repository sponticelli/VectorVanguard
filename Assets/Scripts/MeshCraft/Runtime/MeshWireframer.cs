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
    public static Mesh CreateTriangleWireframeForShader(Mesh mesh)
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

    public static Mesh CreateTriangleWireframeForShader(GameObject targetObject)
    {
      var mesh = MeshHelper.GetMesh(targetObject);

      if (mesh != null) return CreateTriangleWireframeForShader(mesh);

      Debug.LogError("No mesh found on target object!");
      return null;
    }

    public static Mesh CreateTriangleWireframeMesh(Mesh originalMesh)
    {
      var wireframeMesh = new Mesh();

      // Create a list of edge vertices and a corresponding list of triangle indices
      var edgeVertices = new Vector3[originalMesh.triangles.Length * 2];
      var edgeIndices = new int[originalMesh.triangles.Length * 2];

      for (var i = 0; i < originalMesh.triangles.Length; i += 3)
      {
        // Get the indices of the three vertices of the triangle
        var i1 = originalMesh.triangles[i];
        var i2 = originalMesh.triangles[i + 1];
        var i3 = originalMesh.triangles[i + 2];

        // Create a new vertex for each edge of the triangle
        var v1 = originalMesh.vertices[i1];
        var v2 = originalMesh.vertices[i2];
        var v3 = originalMesh.vertices[i3];

        edgeVertices[i * 2] = v1;
        edgeVertices[i * 2 + 1] = v2;
        edgeVertices[i * 2 + 2] = v2;
        edgeVertices[i * 2 + 3] = v3;
        edgeVertices[i * 2 + 4] = v3;
        edgeVertices[i * 2 + 5] = v1;

        // Create new triangle indices for the edge vertices
        edgeIndices[i * 2] = i * 2;
        edgeIndices[i * 2 + 1] = i * 2 + 1;
        edgeIndices[i * 2 + 2] = i * 2 + 2;
        edgeIndices[i * 2 + 3] = i * 2 + 3;
        edgeIndices[i * 2 + 4] = i * 2 + 4;
        edgeIndices[i * 2 + 5] = i * 2 + 5;
      }

      // Assign the edge vertices and indices to the wireframe mesh
      wireframeMesh.vertices = edgeVertices;
      wireframeMesh.SetIndices(edgeIndices, MeshTopology.Lines, 0);

      // Recalculate the bounds and normals of the wireframe mesh
      wireframeMesh.RecalculateBounds();
      wireframeMesh.RecalculateNormals();

      return wireframeMesh;
    }

    public static Mesh CreateTriangleWireframeMesh(GameObject targetObject)
    {
      var mesh = MeshHelper.GetMesh(targetObject);

      if (mesh != null) return CreateTriangleWireframeMesh(mesh);

      Debug.LogError("No mesh found on target object!");
      return null;
    }


    public static Mesh CreateQuadWireframeMesh(Mesh originalMesh)
    {
      var wireframeMesh = new Mesh();

      var edgeVertices = new List<Vector3>();
      var edgeIndices = new List<int>();

      for (var i = 0; i < originalMesh.triangles.Length; i += 3)
      {
        var i1 = originalMesh.triangles[i];
        var i2 = originalMesh.triangles[i + 1];
        var i3 = originalMesh.triangles[i + 2];

        var v1 = originalMesh.vertices[i1];
        var v2 = originalMesh.vertices[i2];
        var v3 = originalMesh.vertices[i3];

        edgeVertices.Add(v1);
        edgeVertices.Add(v2);
        edgeVertices.Add(v2);
        edgeVertices.Add(v3);
        edgeVertices.Add(v3);
        edgeVertices.Add(v1);

        var baseIndex = edgeIndices.Count;
        edgeIndices.Add(baseIndex);
        edgeIndices.Add(baseIndex + 1);
        edgeIndices.Add(baseIndex + 2);
        edgeIndices.Add(baseIndex + 3);
        edgeIndices.Add(baseIndex + 4);
        edgeIndices.Add(baseIndex + 5);
      }

      wireframeMesh.SetVertices(edgeVertices);
      wireframeMesh.SetIndices(edgeIndices.ToArray(), MeshTopology.Lines, 0);

      wireframeMesh.RecalculateBounds();

      return wireframeMesh;
    }


    public static Mesh CreateQuadWireframeMesh(GameObject targetObject)
    {
      var mesh = MeshHelper.GetMesh(targetObject);

      if (mesh != null) return CreateQuadWireframeMesh(mesh);

      Debug.LogError("No mesh found on target object!");
      return null;
    }
  }
}