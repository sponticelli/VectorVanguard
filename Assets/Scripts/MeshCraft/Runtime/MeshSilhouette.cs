using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public static class MeshSilhouette
  {
    public enum Axis
    {
      PositiveX,
      NegativeX,
      PositiveY,
      NegativeY,
      PositiveZ,
      NegativeZ
    }

    public static Mesh CreateSilhouetteMesh(GameObject targetObject, Axis axis)
    {
      var mesh = MeshHelper.GetMesh(targetObject);

      if (mesh != null) return CreateSilhouetteMesh(mesh, axis);

      Debug.LogError("No mesh found on target object!");
      return null;
    }
    
    public static Mesh CreateSilhouetteMesh(Mesh originalMesh, Axis axisDirection)
    {
      var originalVertices = originalMesh.vertices;
      var projectedVertices = new List<Vector2>();

      for (var i = 0; i < originalVertices.Length; i++)
      {
        var projectedVertex = Vector2.zero;
        switch (axisDirection)
        {
          case Axis.PositiveX:
          case Axis.NegativeX:
            projectedVertex = new Vector2(originalVertices[i].y, originalVertices[i].z);
            break;
          case Axis.PositiveY:
          case Axis.NegativeY:
            projectedVertex = new Vector2(originalVertices[i].x, originalVertices[i].z);
            break;
          case Axis.PositiveZ:
          case Axis.NegativeZ:
            projectedVertex = new Vector2(originalVertices[i].x, originalVertices[i].y);
            break;
        }

        if (axisDirection == Axis.NegativeX || axisDirection == Axis.NegativeY || axisDirection == Axis.NegativeZ)
        {
          projectedVertex *= -1;
        }

        projectedVertices.Add(projectedVertex);
      }

      var triangulator = new EarClippingTriangulator(projectedVertices);
      var indices = triangulator.Triangulate();
      
      var pro = new Vector3[projectedVertices.Count];
      for (var i = 0; i < projectedVertices.Count; i++)
      {
        pro[i] = new Vector3(projectedVertices[i].x, projectedVertices[i].y, 0);
      }

      var silhouetteMesh = new Mesh
      {
        vertices = pro,
        triangles = indices
      };
      silhouetteMesh.RecalculateNormals();
      silhouetteMesh.RecalculateBounds();

      return silhouetteMesh;
    }
  }
}