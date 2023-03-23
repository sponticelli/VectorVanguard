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
    [ContextMenu("LiteNinja/Vektor/Bake VectorMeshes")]
    [MenuItem("LiteNinja/Vektor/Bake VectorMeshes")]
    private static void MergeSelectedVectorMeshes(MenuCommand menuCommand)
    {
      var vectorMeshes = new List<AVectorMeshFilter>();

      Material material = null;
      
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
  }
}