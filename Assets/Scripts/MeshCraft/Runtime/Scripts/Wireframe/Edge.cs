using System;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  /// <summary>
  /// Represents an edge connecting two vertices.
  /// </summary>
  public class Edge : IEquatable<Edge>
  {
    /// <summary>
    /// The first vertex of the edge.
    /// </summary>
    public Vertex v1;
    /// <summary>
    /// The second vertex of the edge.
    /// </summary>
    public Vertex v2;
    /// <summary>
    /// The normalized direction vector of the edge.
    /// </summary>
    public Vector3 normalizedDirection;
    /// <summary>
    /// The length of the edge.
    /// </summary>
    public float length;

    /// <summary>
    /// Creates an edge using two vectors.
    /// </summary>
    /// <param name="vert1">The first vector.</param>
    /// <param name="vert2">The second vector.</param>
    public Edge(Vector3 vert1, Vector3 vert2)
    {
      v1 = new Vertex(vert1);
      v2 = new Vertex(vert2);
      normalizedDirection = (vert2 - vert1).normalized;
      length = Vector3.Distance(vert1, vert2);
    }

    /// <summary>
    /// Creates an edge using two vertices.
    /// </summary>
    /// <param name="_v1">The first vertex.</param>
    /// <param name="_v2">The second vertex.</param>
    public Edge(Vertex vertex1, Vertex vertex2)
    {
      v1 = vertex1;
      v2 = vertex2;
      normalizedDirection = (v2 - v1).normalized;
      length = Vector3.Distance(vertex1.position, vertex2.position);
    }

    /// <summary>
    /// Checks whether two edges share a vertex.
    /// </summary>
    /// <param name="_other">The other edge to compare with.</param>
    /// <returns>True if the edges share a vertex, otherwise false.</returns>
    public bool DoShareVertex(Edge _other) => v1.Equals(_other.v1) || v1.Equals(_other.v2) || v2.Equals(_other.v1) || v2.Equals(_other.v2);

    /// <summary>
    /// Checks whether this edge is equal to another edge.
    /// </summary>
    /// <param name="p">The other edge to compare with.</param>
    /// <returns>True if the edges are equal, otherwise false.</returns>
    public bool Equals(Edge p) => v1.Equals(p.v1) && v2.Equals(p.v2) || v1.Equals(p.v2) && v2.Equals(p.v1);

    /// <summary>
    /// Determines the sign of the equality between this edge and another edge.
    /// </summary>
    /// <param name="p">The other edge to compare with.</param>
    /// <returns>1 if the edges are equal, -1 if they are equal but in reverse order, 0 otherwise.</returns>
    public int EqualSign(Edge p)
    {
      if (v1.Equals(p.v1) && v2.Equals(p.v2))
        return 1;
      return v1.Equals(p.v2) && v2.Equals(p.v1) ? -1 : 0;
    }

    /// <summary>
    /// Returns a hash code for this edge.
    /// </summary>
    /// <returns>A hash code for this edge.</returns>
    public override int GetHashCode() => v1.GetHashCode() + v2.GetHashCode();

    /// <summary>
    /// Returns a string representation of this edge.
    /// </summary>
    public override string ToString() => "(" + (object) v1 + ", " + (object) v2 + ")";
  }
}