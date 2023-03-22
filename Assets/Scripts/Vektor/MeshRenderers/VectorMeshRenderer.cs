using UnityEngine;
using Vektor.MeshFilters;

namespace Vektor.MeshRenderers
{
  public class VectorMeshRenderer : MonoBehaviour
  {
    [SerializeField] private AVectorMeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    
    public AVectorMeshFilter VectorMeshFilter
    {
      get => _meshFilter;
      set => _meshFilter = value;
    }

    public MeshRenderer MeshRenderer
    {
      get => _meshRenderer;
      set => _meshRenderer = value;
    }

    public Material[] Materials { get; set; }

    private void Awake()
    {
      SetupComponents();
      Generate(false);
    }
    
    public void SetupComponents()
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