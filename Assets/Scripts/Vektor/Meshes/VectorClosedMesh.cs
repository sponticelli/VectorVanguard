using System;
using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorClosedMesh : VectorMesh
  {
    public Vector3[] Points { get; set; }

    public VectorClosedMesh()
    {
      Points = Array.Empty<Vector3>();
      _mesh = null;
    }
    
    public VectorClosedMesh(Vector3[] points, float lineWidth)
    {
      this.Points = points;
      _lineWidth = lineWidth;
    }
    
    public override Mesh CreateMesh()
    {
      _segments = new Segment[Points.Length];
      for (var i = 0; i < Points.Length; i++)
      {
        _segments[i] = new Segment(Points[i], Points[(i + 1) % Points.Length]);
      }
      return base.CreateMesh();
    }
  }
}