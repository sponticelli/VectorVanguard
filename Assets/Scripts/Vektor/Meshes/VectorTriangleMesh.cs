using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorTriangleMesh : VectorClosedMesh
  {
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _pointC;
    
    public Vector3 pointA
    {
      get => _pointA;
      set => _pointA = value;
    }
    
    public Vector3 pointB
    {
      get => _pointB;
      set => _pointB = value;
    }
    
    public Vector3 pointC
    {
      get => _pointC;
      set => _pointC = value;
    }
    
    public VectorTriangleMesh()
    {
      _pointA = Vector3.zero;
      _pointB = Vector3.zero;
      _pointC = Vector3.zero;
      _mesh = null;
    }
    
    public VectorTriangleMesh(Vector3 pointA, Vector3 pointB, Vector3 pointC, float lineWidth)
    {
      _pointA = pointA;
      _pointB = pointB;
      _pointC = pointC;
      _lineWidth = lineWidth;
    }
    
    public override Mesh CreateMesh()
    {
      Points = new[]
      {
        _pointA,
        _pointB,
        _pointC
      };
      return base.CreateMesh();
    }
  }
}