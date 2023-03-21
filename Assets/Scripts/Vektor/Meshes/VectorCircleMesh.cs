using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorCircleMesh : VectorClosedMesh
  {
    private float _radius;
    private int _chordCount;
    
    public float radius
    {
      get => _radius;
      set => _radius = value;
    }
    
    public int chordCount
    {
      get => _chordCount;
      set => _chordCount = value;
    }
    
    public VectorCircleMesh()
    {
      _radius = 1f;
      _chordCount = 32;
      _mesh = null;
    }
    
    public VectorCircleMesh(float radius, int chordCount, float lineWidth)
    {
      _radius = radius;
      _chordCount = chordCount;
      _lineWidth = lineWidth;
    }
    
    public override Mesh CreateMesh()
    {
      // Create the points
      Points = new Vector3[_chordCount];
      for (var i = 0; i < _chordCount; i++)
      {
        var angle = (float) i / _chordCount * 2 * Mathf.PI;
        Points[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _radius;
      }
      return base.CreateMesh();
    }
  }
}