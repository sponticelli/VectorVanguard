using System;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public struct Edge
  {
    public readonly int VertexIndex1;
    public readonly int VertexIndex2;

    public Edge(int vertexIndex1, int vertexIndex2)
    {
      VertexIndex1 = Mathf.Min(vertexIndex1, vertexIndex2);
      VertexIndex2 = Mathf.Max(vertexIndex1, vertexIndex2);
    }

    public override bool Equals(object obj)
    {
      return obj is Edge edge && VertexIndex1 == edge.VertexIndex1 && VertexIndex2 == edge.VertexIndex2;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(VertexIndex1, VertexIndex2);
    }
  }
}