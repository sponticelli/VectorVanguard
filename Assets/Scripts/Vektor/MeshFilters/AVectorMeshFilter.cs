using UnityEngine;

namespace Vektor.MeshFilters
{
  public abstract class AVectorMeshFilter : MonoBehaviour
  {
    [SerializeField] protected MeshFilter _meshFilter;
    [SerializeField] protected float _lineWidth = 0.1f;
    [SerializeField] protected Material[] _materials;
    
    
    public MeshFilter MeshFilter => _meshFilter;
    public Material[] Materials => _materials;
    public Material Material => _materials.Length > 0 ? _materials[0] : null;


    public void Generate(bool forceUpdate)
    {
      if (_meshFilter == null || _meshFilter.mesh == null)
      {
        forceUpdate = true;
        if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();
      }

      if (forceUpdate)
      {
        GenerateMesh();
      }
    }

    protected abstract void GenerateMesh();

  }
}