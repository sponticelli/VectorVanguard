using UnityEngine;
using Vektor.Meshes;

namespace Vektor.MeshFilters
{
  public class VectorClosedMeshFilter : AVectorMeshFilter
  {
    [SerializeField] private Vector3[] _points;
    
    public Vector3[] Points
    {
      get => _points;
      set => _points = value;
    }
    protected override void GenerateMesh()
    {
      var vectorMesh = new VectorClosedMesh(_points, _lineWidth);
      _meshFilter.mesh = vectorMesh.CreateMesh();
    }
  }
}