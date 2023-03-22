using UnityEngine;
using Vektor.Meshes;

namespace Vektor.MeshFilters
{
  public abstract class AVectorMeshFilter : MonoBehaviour
  {
    [SerializeField] protected MeshFilter _meshFilter;
    
    [SerializeField] protected VectorMesh.JoinType _joinType = VectorMesh.JoinType.None;
    [SerializeField] protected float _lineWidth = 0.1f;
    [SerializeField] protected Material[] _materials;
    
    
    public MeshFilter MeshFilter
    {
      get => _meshFilter;
      set => _meshFilter = value;
    }

    public Material[] Materials
    {
      get => _materials;
      set => _materials = value;
    }

    public Material Material => _materials.Length > 0 ? _materials[0] : null;
    public VectorMesh.JoinType JoinType
    {
      get => _joinType;
      set => _joinType = value;
    }

    public float LineWidth
    {
      get => _lineWidth;
      set => _lineWidth = value;
    }


    public void Generate(bool forceUpdate)
    {
      #if UNITY_EDITOR
      if (_meshFilter == null || _meshFilter.sharedMesh == null)
      {
        forceUpdate = true;
        if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();
      }
      #else
      if (_meshFilter == null || _meshFilter.mesh == null)
      {
        forceUpdate = true;
        if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();
      }
      #endif

      if (forceUpdate)
      {
        GenerateMesh();
      }
    }

    protected abstract void GenerateMesh();

  }
}