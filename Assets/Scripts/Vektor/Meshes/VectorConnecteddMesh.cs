using System;
using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorConnecteddMesh : VectorMesh
  {
    public Vector3[] Points { get; set; }

    public bool IsClosed { get; set; }

    public VectorConnecteddMesh()
    {
      Points = Array.Empty<Vector3>();
      _mesh = null;
    }
    
    public VectorConnecteddMesh(Vector3[] points, float lineWidth)
    {
      this.Points = points;
      _lineWidth = lineWidth;
    }

    public override Mesh CreateMesh()
    {
      var pointCount = IsClosed ? Points.Length : Points.Length - 1;
      _segments = new Segment[pointCount];

      for (var i = 0; i < pointCount; i++)
      {
        _segments[i] = new Segment(Points[i], Points[(i + 1) % Points.Length]);
      }

      return base.CreateMesh();
    }

  }
}