using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LiteNinja.MeshCraft
{
  public class MeshMerger 
  {
    public static Mesh Merge(GameObject gameObject)
    {
      Mesh mergedMesh = null;
      var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
      if (meshFilters.Length > 0)
      {
        var meshes = new Mesh[meshFilters.Length];
        var transforms = new Matrix4x4[meshFilters.Length];
        var i = 0;
        while (i < meshFilters.Length)
        {
          meshes[i] = meshFilters[i].sharedMesh;
          transforms[i] = meshFilters[i].transform.localToWorldMatrix;
          i++;
        }

        mergedMesh = Merge(meshes, transforms);
      }

      var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
      if (skinnedMeshRenderers.Length > 0)
      {
        var numMeshes = mergedMesh != null ? 1 + skinnedMeshRenderers.Length : skinnedMeshRenderers.Length;
        var meshes = new Mesh[numMeshes];
        var transforms = new Matrix4x4[numMeshes];
        var i = 0;
        if (mergedMesh != null)
        {
          meshes[0] = mergedMesh;
          transforms[0] = Matrix4x4.identity;
          i++;
        }
        while (i < numMeshes)
        {
          meshes[i] = skinnedMeshRenderers[i - 1].sharedMesh;
          transforms[i] = skinnedMeshRenderers[i - 1].transform.localToWorldMatrix;
          i++;
        }
        
        mergedMesh = Merge(meshes, transforms);
      }

      return mergedMesh != null ? mergedMesh : new Mesh();
    }

    public static Mesh Merge(Mesh[] meshes)
    {
      var transforms = new Matrix4x4[meshes.Length];
      var i = 0;
      while (i < meshes.Length)
      {
        transforms[i] = Matrix4x4.identity;
        i++;
      }
      return Merge(meshes, transforms);
    }
    
    public static Mesh Merge(Mesh[] meshes, Matrix4x4[] transforms)
    {
      var combine = new CombineInstance[meshes.Length];
      var i = 0;
      while (i < meshes.Length)
      {
        combine[i].mesh = meshes[i];
        combine[i].transform = transforms[i];
        i++;
      }
      // Generate a new mesh and return it
      var mesh = new Mesh();
      mesh.CombineMeshes(combine);
      return mesh;
    }
  }
}

