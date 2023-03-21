using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorSquareMesh : VectorQuadrilateralMesh
  {
    private float _sideLength;
    
    public float sideLength
    {
      get => _sideLength;
      set => _sideLength = value;
    }
    
    public VectorSquareMesh()
    {
      pointA = Vector3.zero;
      pointB = Vector3.zero;
      pointC = Vector3.zero;
      pointD = Vector3.zero;
      _sideLength = 0f;
    }
    
    public VectorSquareMesh(float sideLength, float lineWidth)
    {
      // Center the square at the origin
      pointA = new Vector3(-sideLength / 2f, -sideLength / 2f, 0f);
      pointB = new Vector3(sideLength / 2f, -sideLength / 2f, 0f);
      pointC = new Vector3(sideLength / 2f, sideLength / 2f, 0f);
      pointD = new Vector3(-sideLength / 2f, sideLength / 2f, 0f);
      _sideLength = sideLength;
    }
  }
}