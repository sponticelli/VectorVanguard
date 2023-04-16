using UnityEngine;

namespace VectorVanguard.Utils.Meshes
{

  [RequireComponent(typeof(MeshFilter))]
  public class StaggeredRectangleMesh : MonoBehaviour
  {
    [SerializeField] private MeshFilter _meshFilter;
    
    [SerializeField][Range(2,255)] private int _numVerticesX = 64;
    [SerializeField][Range(2,255)] private int _numVerticesY = 32;
    [SerializeField] private float _edgeSize = 1f;

    public MeshFilter MeshFilter
    {
      get => _meshFilter;
      set => _meshFilter = value;
    }
    
    
    private void Awake()
    {
      Init();
    }

    private void Start()
    {
      if (_meshFilter.mesh == null)
      {
        Generate();
      }
    }

    public void Generate()
    {
#if UNITY_EDITOR
      if (!Application.isPlaying)
      {
        _meshFilter.sharedMesh = MeshHelper.GenerateStaggeredGridMesh(_numVerticesX, _numVerticesY, _edgeSize);
      }
      else
      {
        _meshFilter.mesh = MeshHelper.GenerateStaggeredGridMesh(_numVerticesX, _numVerticesY, _edgeSize);
      }
#else
      _meshFilter.mesh = MeshHelper.GenerateStaggeredGridMesh(_numVerticesX, _numVerticesY, _edgeSize);
#endif
    }


    public void Init()
    {
      if (_meshFilter == null)
      {
        _meshFilter = GetComponent<MeshFilter>();
      }

      if (_numVerticesX < 2)
      {
        _numVerticesX = 2;
      }

      if (_numVerticesY < 2)
      {
        _numVerticesY = 2;
      }
    }
  }
}