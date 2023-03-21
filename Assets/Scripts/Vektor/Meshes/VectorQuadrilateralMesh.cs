using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorQuadrilateralMesh : VectorClosedMesh
  {
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _pointC;
    private Vector3 _pointD;
    
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
    
    public Vector3 pointD
    {
      get => _pointD;
      set => _pointD = value;
    }
    
    public VectorQuadrilateralMesh()
    {
      _pointA = Vector3.zero;
      _pointB = Vector3.zero;
      _pointC = Vector3.zero;
      _pointD = Vector3.zero;
    }
    
    public VectorQuadrilateralMesh(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, float lineWidth)
    {
      _pointA = pointA;
      _pointB = pointB;
      _pointC = pointC;
      _pointD = pointD;
      _lineWidth = lineWidth;
    }
    
    public override Mesh CreateMesh()
    {
      Points = new[]
      {
        _pointA,
        _pointB,
        _pointC,
        _pointD
      };
      
      return base.CreateMesh();
      
    }
  }
}