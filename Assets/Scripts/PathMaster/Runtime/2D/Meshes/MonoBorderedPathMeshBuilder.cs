using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  /// <summary>
  /// A mono behaviour that generates a mesh for a path with a constant width using a CenteredPathMeshBuilder
  /// </summary>
  public class MonoBorderedPathMeshBuilder : MonoBehaviour
  {
    
    
    [Header("References")]
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MonoPath2D _monoPath2D;
    
    [Header("Parameters")]
    [SerializeField]  private float _width = 1f;
    [SerializeField][Range(0.01f, 10f)]  private float _sppacing = 0.01f;
    [SerializeField][Range(0.01f, 10f)]  private float _borderWidth = 0.1f;
    
    private BorderedPathMeshBuilder _meshBuilder;
    
    private void Awake()
    {
      _meshFilter = GetComponent<MeshFilter>();
      _meshRenderer = GetComponent<MeshRenderer>();
      Generate();
    }

    public void Generate()
    {
      if (_meshBuilder == null)
      {
        _meshBuilder = new BorderedPathMeshBuilder(_sppacing, _width, _borderWidth);
      }
      else
      {
        _meshBuilder.Width = _width;
        _meshBuilder.Spacing = _sppacing;
        _meshBuilder.BorderWidth = _borderWidth;
      }
#if UNITY_EDITOR
      _meshFilter.sharedMesh = _meshBuilder.BuildMesh(_monoPath2D.Path);
#else
      _meshFilter.mesh = _meshBuilder.BuildMesh(_monoPath2D.Path);  
#endif

    }
  }
}