using UnityEngine;

namespace Vektor.Meshes
{
  public class VectorRectangleMesh : VectorQuadrilateralMesh
  {
    private float _width;
    private float _height;
    
    public float width
    {
      get => _width;
      set => _width = value;
    }
    
    public float height
    {
      get => _height;
      set => _height = value;
    }
    
    public VectorRectangleMesh()
    {
      pointA = Vector3.zero;
      pointB = Vector3.zero;
      pointC = Vector3.zero;
      pointD = Vector3.zero;
      _width = 0f;
      _height = 0f;
    }
    
    public VectorRectangleMesh(float width, float height, float lineWidth)
    {
      // Center the rectangle at the origin
      pointA = new Vector3(-width / 2f, -height / 2f, 0f);
      pointB = new Vector3(width / 2f, -height / 2f, 0f);
      pointC = new Vector3(width / 2f, height / 2f, 0f);
      pointD = new Vector3(-width / 2f, height / 2f, 0f);
      _width = width;
      _height = height;
    }
  }
}