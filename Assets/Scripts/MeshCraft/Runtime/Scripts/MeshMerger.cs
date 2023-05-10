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
    
    public static void MergeMeshes(ref Mesh mesh1, Mesh mesh2, string name, MeshAttributes meshAttributes)
    {
      var capacity = mesh1.vertexCount + mesh2.vertexCount;
      meshAttributes ??= new MeshAttributes();
      var vertexList = new List<Vector3>(capacity);
      var triangleList = new List<int[]>(mesh1.triangles.Length + mesh2.triangles.Length);
      var normalList = new List<Vector3>(meshAttributes.UseNormals ? capacity : 0);
      var tangentList = new List<Vector4>(meshAttributes.UseTangents ? capacity : 0);
      var uvs1 = new List<Vector4>(capacity);
      var uvs2 = new List<Vector4>(meshAttributes.UseUV2 ? capacity : 0);
      var uvs3 = new List<Vector4>(meshAttributes.UseUV3 ? capacity : 0);
      var uvs4 = new List<Vector4>(meshAttributes.UseUV4 ? capacity : 0);
      var colorList = new List<Color>(meshAttributes.UseColors ? capacity : 0);
      var boneWeightList = new List<BoneWeight>(meshAttributes.UseSkin ? capacity : 0);
      vertexList.AddRange(mesh1.vertices);
      vertexList.AddRange(mesh2.vertices);
      var vertexCount = mesh1.vertexCount;
      for (var subMesh = 0; subMesh < mesh2.subMeshCount; ++subMesh)
      {
        var triangles = mesh2.GetTriangles(subMesh);
        for (var index = 0; index < triangles.Length; ++index)
          triangles[index] += vertexCount;
        var intList = new List<int>(mesh1.GetTriangles(subMesh).Length + triangles.Length);
        intList.AddRange(mesh1.GetTriangles(subMesh));
        intList.AddRange(triangles);
        triangleList.Add(intList.ToArray());
      }

      if (meshAttributes.UseNormals)
      {
        normalList.AddRange(mesh1.normals);
        normalList.AddRange(mesh2.normals);
      }

      if (meshAttributes.UseTangents)
      {
        tangentList.AddRange(mesh1.tangents);
        tangentList.AddRange(mesh2.tangents);
      }

      var vector4List2 = new List<Vector4>();
      mesh1.GetUVs(0, vector4List2);
      uvs1.AddRange(vector4List2);
      var vector4List3 = new List<Vector4>();
      mesh2.GetUVs(0, vector4List3);
      uvs1.AddRange(vector4List3);
      if (meshAttributes.UseUV2)
      {
        var vector4List4 = new List<Vector4>();
        mesh1.GetUVs(1, vector4List4);
        uvs2.AddRange(vector4List4);
        var vector4List5 = new List<Vector4>();
        mesh2.GetUVs(1, vector4List5);
        uvs2.AddRange(vector4List5);
      }

      if (meshAttributes.UseUV3)
      {
        var vector4List6 = new List<Vector4>();
        mesh1.GetUVs(2, vector4List6);
        uvs3.AddRange(vector4List6);
        var vector4List7 = new List<Vector4>();
        mesh2.GetUVs(2, vector4List7);
        uvs3.AddRange(vector4List7);
      }

      if (meshAttributes.UseUV4)
      {
        var vector4List8 = new List<Vector4>();
        mesh1.GetUVs(3, vector4List8);
        uvs4.AddRange(vector4List8);
        var vector4List9 = new List<Vector4>();
        mesh2.GetUVs(3, vector4List9);
        uvs4.AddRange(vector4List9);
      }

      if (meshAttributes.UseColors)
      {
        colorList.AddRange(mesh1.colors);
        colorList.AddRange(mesh2.colors);
      }

      if (meshAttributes.UseSkin)
      {
        boneWeightList.AddRange(mesh1.boneWeights);
        boneWeightList.AddRange(mesh2.boneWeights);
      }

      if (vertexList.Count > 65000)
      {
        Debug.LogWarning("Wireframed mesh \"" + name + "\" has " + vertexList.Count +
                         " vertices. A mesh may not have more than 65000 vertices.\n");
        mesh1.Clear();
        mesh1 = null;
      }
      else
      {
        mesh1.vertices = vertexList.ToArray();
        mesh1.subMeshCount = triangleList.Count;
        for (var index = 0; index < triangleList.Count; ++index)
          mesh1.SetTriangles(triangleList[index], index);
        mesh1.normals = normalList.ToArray();
        mesh1.tangents = tangentList.ToArray();
        mesh1.SetUVs(0, uvs1);
        mesh1.SetUVs(1, uvs2);
        mesh1.SetUVs(2, uvs3);
        mesh1.SetUVs(3, uvs4);
        mesh1.colors = colorList.ToArray();
        mesh1.boneWeights = boneWeightList.ToArray();
        mesh1.bindposes = mesh1.bindposes;
      }
    }
  }
}

