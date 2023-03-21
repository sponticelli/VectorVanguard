using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorLineMesh : VectorMesh
  {
    private Vector3 _start;
    private Vector3 _end;
    
    public Vector3 start
    {
      get => _start;
      set => _start = value;
    }
    
    public Vector3 end
    {
      get => _end;
      set => _end = value;
    }
    
    public VectorLineMesh()
    {
      _start = Vector3.zero;
      _end = Vector3.zero;
      _mesh = null;
    }
    
    public VectorLineMesh(Vector3 start, Vector3 end, float lineWidth)
    {
      _start = start;
      _end = end;
      _lineWidth = lineWidth;
    }
    
    public override Mesh CreateMesh()
    {
      _segments = new[]
      {
        new Segment(_start, _end)
      };
      return base.CreateMesh();
    }
  }
}