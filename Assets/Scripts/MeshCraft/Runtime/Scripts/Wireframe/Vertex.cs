using System;
using UnityEngine;

namespace LiteNinja.MeshCraft.Wireframe
{
  /// <summary>
  /// Represents a vertex in a 3D mesh, storing its position as a long integer for precision.
  /// </summary>
  public class Vertex : IEquatable<Vertex>
  {
    private const float ScaleFactor = 100000;
    public readonly Vector3 position;
    private long x;
    private long y;
    private long z;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> class.
    /// </summary>
    /// <param name="vertex">The Vector3 position of the vertex.</param>
    public Vertex(Vector3 vertex)
    {
      x = (long)(vertex.x * ScaleFactor);
      y = (long)(vertex.y * ScaleFactor);
      z = (long)(vertex.z * ScaleFactor);
      position = new Vector3(x, y, z) / ScaleFactor;
    }

    /// <summary>
    /// Calculates the hash code for this vertex based on its x, y, and z coordinates.
    /// </summary>
    /// <returns>The hash code for this vertex.</returns>
    public override int GetHashCode() =>
      ((unchecked(17 * 576284739) + x.GetHashCode()) * 576284739 + y.GetHashCode()) * 576284739 +
      z.GetHashCode();

    /// <summary>
    /// Determines whether the specified <see cref="Vertex"/> is equal to the current <see cref="Vertex"/>.
    /// </summary>
    /// <param name="p">The <see cref="Vertex"/> to compare with the current <see cref="Vertex"/>.</param>
    /// <returns>true if the specified <see cref="Vertex"/> is equal to the current <see cref="Vertex"/>; otherwise, false.</returns>
    public bool Equals(Vertex p) => GetHashCode() == p.GetHashCode();

    /// <summary>
    /// Subtracts the position of one vertex from another, returning the difference as a Vector3.
    /// </summary>
    /// <param name="a">The first vertex.</param>
    /// <param name="b">The second vertex.</param>
    /// <returns>A Vector3 representing the difference between the positions of the two vertices.</returns>
    public static Vector3 operator -(Vertex a, Vertex b) => a.position - b.position;

    /// <summary>
    /// Returns a string representation of the vertex, displaying its x, y, and z coordinates.
    /// </summary>
    /// <returns>A string representation of the vertex.</returns>
    public override string ToString() => $"{x},{y},{z}";
  }
}