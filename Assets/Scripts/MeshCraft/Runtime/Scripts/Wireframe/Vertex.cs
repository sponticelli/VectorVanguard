using System;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public class Vertex : IEquatable<Vertex>
  {
    private const int scaleFactor = 100000;
    public readonly Vector3 position;
    private long x;
    private long y;
    private long z;

    public Vertex(Vector3 _v)
    {
      x = (long) (_v.x * 100000.0);
      y = (long) (_v.y * 100000.0);
      z = (long) (_v.z * 100000.0);
      position = new Vector3(x, y, z) / 100000f;
    }

    public override int GetHashCode() => ((unchecked(17 * 576284739) + this.x.GetHashCode()) * 576284739 + this.y.GetHashCode()) * 576284739 + this.z.GetHashCode();

    

    public bool Equals(Vertex p) => GetHashCode() == p.GetHashCode();

    public static Vector3 operator -(Vertex a, Vertex b) => a.position - b.position;

    public override string ToString() => $"{x},{y},{z}";
  }
}