using System.Collections.Generic;
using UnityEngine;

namespace Vektor.MeshFilters
{
  public class VectorCombineMeshFilter : AVectorMeshFilter
  {
    [SerializeField] private AVectorMeshFilter[] _meshFilters;
    

    protected override void GenerateMesh()
    {
      if (_meshFilters == null || _meshFilters.Length == 0) return;
      var mesh = new Mesh();
      var combine = new CombineInstance[_meshFilters.Length];
      var mergeSubMeshes = true;
      var materials = new List<Material>();

      if (_meshFilters[0].Materials.Length > 0)
      {
        mergeSubMeshes = false;
        materials.AddRange(_meshFilters[0].Materials);
      }
      else
      {
        materials.Add(_meshFilters[0].Material);
      }

      for (var i = 0; i < _meshFilters.Length; i++)
      {
        combine[i].mesh = _meshFilters[i].MeshFilter.mesh;
        combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;

        var shouldMerge = _meshFilters[i].Materials.Length == 1;
        foreach (var material in _meshFilters[i].Materials)
        {
          if (materials.Contains(material)) continue;
          materials.Add(material);
          shouldMerge = false;
        }
        mergeSubMeshes &= shouldMerge;
      }
      mesh.CombineMeshes(combine, mergeSubMeshes);
      _meshFilter.mesh = mesh;
    }
  }
}