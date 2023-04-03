using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vektor.Meshes;
using Vektor.MeshFilters;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  public class VectorMeshMergerEditor
  {
    [ContextMenu("LiteNinja/Vektor/Bake VectorMeshes to 3D (mesh)")]
    [MenuItem("LiteNinja/Vektor/Bake VectorMeshes to 3D (mesh)")]
    private static void MergeSelectedVectorMeshes3D(MenuCommand menuCommand)
    {
      var vectorMeshes = GetVectorMeshes(out var material);

      if (vectorMeshes.Count > 0)
      {
        var vectorMeshesToMerge = vectorMeshes.Select(vm => vm.MeshFilter.sharedMesh).ToList();


        var mergedMesh = VectorMeshUtils.MergeMeshes(vectorMeshesToMerge);

        // Create a new GameObject to display the merged mesh
        var mergedMeshObj = new GameObject("MergedVectorMesh");
        var meshFilter = mergedMeshObj.AddComponent<MeshFilter>();
        var meshRenderer = mergedMeshObj.AddComponent<MeshRenderer>();

        meshFilter.mesh = mergedMesh;
        meshRenderer.material = material != null ? material : new Material(Shader.Find("Standard"));
      }
      else
      {
        Debug.LogWarning("Select at least one GameObjects with VectorMeshFilter components to merge.");
      }
    }

    private static List<AVectorMeshFilter> GetVectorMeshes(out Material material)
    {
      var vectorMeshes = new List<AVectorMeshFilter>();

      material = null;

      foreach (var gameObject in Selection.gameObjects)
      {
        var vectorMesh = gameObject.GetComponent<AVectorMeshFilter>();
        if (vectorMesh == null)
        {
          continue;
        }

        vectorMeshes.Add(vectorMesh);
        if (material != null) continue;
        if (vectorMesh.Materials.Length > 0)
        {
          material = vectorMesh.Materials[0];
        }
      }

      return vectorMeshes;
    }
  }
}