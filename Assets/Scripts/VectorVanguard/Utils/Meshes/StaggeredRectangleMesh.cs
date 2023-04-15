using System;
using UnityEngine;
using VectorVanguard.Utils;
using Random = UnityEngine.Random;

namespace VectorVanguard.Utils.Meshes
{

  [RequireComponent(typeof(MeshFilter))]
  public class StaggeredRectangleMesh : MonoBehaviour
  {
    [SerializeField] private MeshFilter _meshFilter;
    
    [SerializeField][Range(2,255)] private int _numVerticesX = 64;
    [SerializeField][Range(2,255)] private int _numVerticesY = 32;
    [SerializeField] private float _edgeSize = 1f;

    private void Awake()
    {
      Init();
      if (_meshFilter.mesh == null)
      {
        _meshFilter.mesh = MeshHelper.GenerateStaggeredGridMesh(_numVerticesX, _numVerticesY, _edgeSize);
      }
    }
    
    
    private void OnValidate()
    {
      Init();
      _meshFilter.sharedMesh = MeshHelper.GenerateStaggeredGridMesh(_numVerticesX, _numVerticesY, _edgeSize);
    }

    private void Init()
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