using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vektor.Meshes
{
  public static class VectorMeshUtils
  {
    public static Mesh MergeVectorMeshes(List<VectorMesh> vectorMeshes)
    {
      var mergedMesh = new Mesh();
      var vertices = new List<Vector3>();
      var triangles = new List<int>();

      foreach (var vectorMesh in vectorMeshes)
      {
        var mesh = vectorMesh.CreateMesh();

        var vertexOffset = vertices.Count;

        vertices.AddRange(mesh.vertices);

        triangles.AddRange(mesh.triangles.Select(t => t + vertexOffset));
      }

      mergedMesh.vertices = vertices.ToArray();
      mergedMesh.triangles = triangles.ToArray();
      mergedMesh.RecalculateNormals();
      mergedMesh.RecalculateBounds();

      return mergedMesh;
    }
    
    public static Mesh MergeMeshes(List<Mesh> meshes)
    {
      var mergedMesh = new Mesh();
      var vertices = new List<Vector3>();
      var triangles = new List<int>();

      foreach (var mesh in meshes)
      {
        var vertexOffset = vertices.Count;

        vertices.AddRange(mesh.vertices);

        triangles.AddRange(mesh.triangles.Select(t => t + vertexOffset));
      }

      mergedMesh.vertices = vertices.ToArray();
      mergedMesh.triangles = triangles.ToArray();
      mergedMesh.RecalculateNormals();
      mergedMesh.RecalculateBounds();

      return mergedMesh;
    }
  }
}