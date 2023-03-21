using UnityEngine;
using Vektor.MeshFilters;

namespace Vektor.MeshRenderers
{
  public class VectorMeshRenderer : MonoBehaviour
  {
    [SerializeField] private AVectorMeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    
    public AVectorMeshFilter MeshFilter => _meshFilter;
    public MeshRenderer MeshRenderer => _meshRenderer;

    private void Awake()
    {
      SetupComponents();
      Generate();
    }
    
    private void SetupComponents()
    {
      if (_meshFilter == null) _meshFilter = GetComponent<AVectorMeshFilter>();
      if (_meshRenderer != null) return;
      _meshRenderer = GetComponent<MeshRenderer>();
      if (_meshRenderer == null) _meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void Generate(bool forceUpdate = false)
    {
      _meshFilter.Generate(forceUpdate);
      _meshRenderer.materials = _meshFilter.Materials;
    }
  }
}