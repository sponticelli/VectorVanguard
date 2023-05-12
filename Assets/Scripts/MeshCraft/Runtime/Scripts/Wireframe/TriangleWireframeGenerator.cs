using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LiteNinja.MeshCraft.Wireframe
{
  public class TriangleWireframeGenerator
  {
#region Public Methods

    public static Mesh Generate(Mesh origMesh) => Generate(origMesh, new MeshAttributes());

    public static Mesh Generate(Mesh origMesh, bool uv2, bool uv3, bool normal, bool tangent,
      bool color, bool skin = false)
    {
      return Generate(origMesh, uv2, uv3, false, normal, tangent, color, skin);
    }

    public static Mesh Generate(Mesh origMesh, bool uv2, bool uv3, bool uv4, bool normal, bool tangent,
      bool color, bool skin)
    {
      return Generate(origMesh, new MeshAttributes
      {
        UseUV2 = uv2,
        UseUV3 = uv3,
        UseUV4 = uv4,
        UseNormals = normal,
        UseTangents = tangent,
        UseColors = color,
        UseSkin = skin
      });
    }

    public static Mesh GenerateFast(Mesh origMesh) => GenerateFast(origMesh, new MeshAttributes());

    public static Mesh GenerateFast(Mesh origMesh, bool uv2, bool uv3, bool uv4, bool normal, bool tangent,
      bool color, bool skin)
    {
      return GenerateFast(origMesh, new MeshAttributes
      {
        UseUV2 = uv2,
        UseUV3 = uv3,
        UseUV4 = uv4,
        UseNormals = normal,
        UseTangents = tangent,
        UseColors = color,
        UseSkin = skin
      });
    }

    
#endregion

#region Private Methods

    
    /// <summary>
    /// a good triangle is a triangle whose vertices have Barycentric coordinates that sum up to 1 for each of the
    /// x, y, and z components. This means that the Barycentric coordinates of the vertices in a good triangle form a
    /// valid barycentric representation
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="badTriangles"></param>
    /// <param name="goodTriangles"></param>
    /// <returns></returns>
    private static bool DivideMeshOnBadAndGoodTriangles(Mesh mesh, out List<int[]> badTriangles,
      out List<int[]> goodTriangles)
    {
      var flag = false;
      badTriangles = new List<int[]>();
      goodTriangles = new List<int[]>();
      var uvs = new List<Vector4>();
      mesh.GetUVs(0, uvs);
      for (var subMesh = 0; subMesh < mesh.subMeshCount; ++subMesh)
      {
        var subMeshGoodTriangles = new List<int>();
        var subMeshBadTriangles = new List<int>();
        var triangles = mesh.GetTriangles(subMesh);
        for (var index = 0; index < triangles.Length; index += 3)
        {
          var barycentric1 = BarycentricCoordsHelper.UVToBarycentric(uvs[triangles[index]]);
          var barycentric2 = BarycentricCoordsHelper.UVToBarycentric(uvs[triangles[index + 1]]);
          var barycentric3 = BarycentricCoordsHelper.UVToBarycentric(uvs[triangles[index + 2]]);
          var xSum = (float)(barycentric1.x + barycentric2.x + barycentric3.x);
          var ySum = (float)(barycentric1.y + barycentric2.y + barycentric3.y);
          var zSum = (float)(barycentric1.z + barycentric2.z + barycentric3.z);
          if (xSum != 1.0 || ySum != 1.0 || zSum != 1.0)
          {
            subMeshGoodTriangles.Add(triangles[index]);
            subMeshGoodTriangles.Add(triangles[index + 1]);
            subMeshGoodTriangles.Add(triangles[index + 2]);
            flag = true;
          }
          else
          {
            subMeshBadTriangles.Add(triangles[index]);
            subMeshBadTriangles.Add(triangles[index + 1]);
            subMeshBadTriangles.Add(triangles[index + 2]);
          }
        }

        badTriangles.Add(subMeshGoodTriangles.ToArray());
        goodTriangles.Add(subMeshBadTriangles.ToArray());
      }

      return flag;
    }

    /// <summary>
    /// Builds a new mesh by extracting the specified triangles from the original mesh, preserving selected mesh attributes.
    /// </summary>
    /// <param name="origMesh">The original mesh to extract triangles from.</param>
    /// <param name="trianglesIndices">A list of triangle indices to extract from the original mesh.</param>
    /// <param name="rebuildBarycentricCoords">Indicates whether to rebuild barycentric coordinates for the new mesh.</param>
    /// <param name="meshAttributes">An object specifying which mesh attributes (normals, tangents, UVs, etc.) to preserve in the new mesh.</param>
    /// <returns>A new mesh containing only the specified triangles, with selected mesh attributes preserved.</returns>
    private static Mesh BuildMeshFromTriangles(Mesh origMesh, List<int[]> trianglesIndices,
      bool rebuildBarycentricCoords,
      MeshAttributes meshAttributes)
    {
      var mesh = new Mesh
      {
        name = origMesh.name
      };
      meshAttributes ??= new MeshAttributes();
      var vertices = origMesh.vertices;
      var normals = origMesh.normals;
      var tangents = origMesh.tangents;
      var uvs1 = new List<Vector4>();
      origMesh.GetUVs(0, uvs1);

      var uvs2 = new List<Vector4>();
      if (meshAttributes.UseUV2) origMesh.GetUVs(1, uvs2);

      var uvs3 = new List<Vector4>();
      if (meshAttributes.UseUV3) origMesh.GetUVs(2, uvs3);

      var uvs4 = new List<Vector4>();
      if (meshAttributes.UseUV4) origMesh.GetUVs(3, uvs4);

      var colors = origMesh.colors;
      var boneWeights = origMesh.boneWeights;
      var bindposes = origMesh.bindposes;
      var vertexList = new List<Vector3>();
      var triangleList = new List<int[]>();
      var normalList = new List<Vector3>();
      var tangentList = new List<Vector4>();
      var uv1ForNewMesh = new List<Vector4>();
      var uvs5 = new List<Vector4>();
      var uvs6 = new List<Vector4>();
      var uvs7 = new List<Vector4>();
      var colorList = new List<Color>();
      var boneWeightList = new List<BoneWeight>();
      var remappedVertexIndex = new Dictionary<int, int>();

      var useNormals = meshAttributes.UseNormals;
      if (useNormals) useNormals = origMesh.normals != null && origMesh.normals.Length == origMesh.vertexCount;

      var useTangents = meshAttributes.UseTangents;
      if (useTangents) useTangents = origMesh.tangents != null && origMesh.tangents.Length == origMesh.vertexCount;

      if (uvs1 == null || uvs1.Count != origMesh.vertexCount)
      {
        uvs1 = new List<Vector4>(origMesh.vertexCount);
        for (var index = 0; index < origMesh.vertexCount; ++index)
        {
          uvs1.Add(Vector4.zero);
        }
      }

      var useUV2 = meshAttributes.UseUV2;
      if (useUV2) useUV2 = origMesh.uv2 != null && origMesh.uv2.Length == origMesh.vertexCount;

      var useUV3 = meshAttributes.UseUV3;
      if (useUV3) useUV3 = origMesh.uv3 != null && origMesh.uv3.Length == origMesh.vertexCount;

      var useUV4 = meshAttributes.UseUV4;
      if (useUV4) useUV4 = origMesh.uv4 != null && origMesh.uv4.Length == origMesh.vertexCount;

      var useColors = meshAttributes.UseColors;
      if (useColors) useColors = origMesh.colors != null && origMesh.colors.Length == origMesh.vertexCount;

      var useSkin = meshAttributes.UseSkin;
      if (useSkin) useSkin = origMesh.boneWeights != null && origMesh.boneWeights.Length == origMesh.vertexCount;

      var num = 0;
      foreach (var t in trianglesIndices)
      {
        var remappedTriangleIndices = new List<int>();
        foreach (var vertexIndex in t)
        {
          if (remappedVertexIndex.ContainsKey(vertexIndex))
          {
            remappedTriangleIndices.Add(remappedVertexIndex[vertexIndex]);
          }
          else
          {
            vertexList.Add(vertices[vertexIndex]);
            remappedTriangleIndices.Add(num++);
            if (useNormals) normalList.Add(normals[vertexIndex]);
            if (useTangents) tangentList.Add(tangents[vertexIndex]);
            uv1ForNewMesh.Add(uvs1[vertexIndex]);
            if (useUV2) uvs5.Add(uvs2[vertexIndex]);
            if (useUV3) uvs6.Add(uvs3[vertexIndex]);
            if (useUV4) uvs7.Add(uvs4[vertexIndex]);
            if (useColors) colorList.Add(colors[vertexIndex]);
            if (useSkin) boneWeightList.Add(boneWeights[vertexIndex]);
            remappedVertexIndex.Add(vertexIndex, vertexList.Count - 1);
          }
        }

        triangleList.Add(remappedTriangleIndices.ToArray());
      }

      mesh.vertices = vertexList.ToArray();
      mesh.subMeshCount = triangleList.Count;
      for (var index = 0; index < triangleList.Count; ++index)
      {
        mesh.SetTriangles(triangleList[index], index);
      }

      mesh.normals = normalList.ToArray();
      mesh.tangents = tangentList.ToArray();
      if (rebuildBarycentricCoords)
      {
        var triangles = mesh.triangles;
        var barycentricMass = BarycentricCoordsHelper.GenerateBarycentricCoords(mesh.vertexCount, ref triangles);
        BarycentricCoordsHelper.BakeBarycentricToUV(uv1ForNewMesh, barycentricMass);
      }

      mesh.SetUVs(0, uv1ForNewMesh);
      mesh.SetUVs(1, uvs5);
      mesh.SetUVs(2, uvs6);
      mesh.SetUVs(3, uvs7);
      mesh.colors = colorList.ToArray();
      if (!useSkin) return mesh;
      mesh.boneWeights = boneWeightList.ToArray();
      mesh.bindposes = bindposes;
      return mesh;
    }

    /// <summary>
    /// Clones a mesh and selectively preserves its attributes based on the provided MeshAttributes object.
    /// </summary>
    /// <param name="origMesh">The original mesh to clone.</param>
    /// <param name="meshAttributes">An object specifying which mesh attributes (normals, tangents, UVs, etc.) to preserve in the cloned mesh.</param>
    /// <returns>A new mesh that is a clone of the original mesh with selected mesh attributes preserved.</returns>
    private static Mesh CloneMesh(Mesh origMesh, MeshAttributes meshAttributes)
    {
      if (origMesh == null)
      {
        Debug.LogError("Can not generate wireframe from empty mesh.\n");
        return null;
      }

      var mesh = Object.Instantiate(origMesh);
      mesh.name = origMesh.name;
      for (var subMesh = 0; subMesh < origMesh.subMeshCount; ++subMesh)
      {
        mesh.SetTriangles(origMesh.GetTriangles(subMesh), subMesh);
      }

      var triangles = mesh.triangles;
      var barycentricCoords = BarycentricCoordsHelper.GenerateBarycentricCoords(mesh.vertexCount, ref triangles);
      var uv1List = new List<Vector4>();
      mesh.GetUVs(0, uv1List);
      if (uv1List == null || uv1List.Count != mesh.vertexCount)
      {
        uv1List = new List<Vector4>(mesh.vertexCount);
        for (var index = 0; index < mesh.vertexCount; ++index)
        {
          uv1List.Add(Vector4.zero);
        }
      }

      BarycentricCoordsHelper.BakeBarycentricToUV(uv1List, barycentricCoords);
      mesh.SetUVs(0, uv1List);
      if (!meshAttributes.UseUV2) mesh.uv2 = Array.Empty<Vector2>();
      if (!meshAttributes.UseUV3) mesh.uv3 = Array.Empty<Vector2>();
      if (!meshAttributes.UseUV4) mesh.uv4 = Array.Empty<Vector2>();
      if (!meshAttributes.UseNormals) mesh.normals = Array.Empty<Vector3>();
      if (!meshAttributes.UseTangents) mesh.tangents = Array.Empty<Vector4>();
      if (!meshAttributes.UseColors) mesh.colors = Array.Empty<Color>();
      if (meshAttributes.UseSkin) return mesh;

      mesh.boneWeights = Array.Empty<BoneWeight>();
      mesh.bindposes = Array.Empty<Matrix4x4>();
      return mesh;
    }

    /// <summary>
    /// Generates a wireframe mesh from a given original mesh and mesh attributes
    /// </summary>
    /// <param name="originalMesh">The original mesh to generate a wireframe from</param>
    /// <param name="meshAttributes">Mesh attributes for the wireframe mesh</param>
    /// <returns>A wireframe mesh generated from the original mesh</returns>
    /// <remarks>
    /// This method first clones the original mesh as a flat mesh and then iteratively subdivides the mesh until all triangles are good.
    /// The method creates two lists of triangles, good and bad, and builds new meshes from these lists.
    /// The good mesh is added to a list of meshes and the bad mesh is iteratively subdivided until all triangles are good.
    /// If the bad mesh has triangles with wrong indices, a warning is logged and the bad mesh is discarded.
    /// The resulting meshes are merged into a final wireframe mesh.
    /// </remarks>
    private static Mesh Generate(Mesh originalMesh, MeshAttributes meshAttributes)
    {
      if (originalMesh == null || originalMesh.vertexCount < 3 || originalMesh.triangles.Length == 0)
      {
        Debug.LogError("Can not generate wireframe from empty mesh.\n");
        return null;
      }

      var clonedMesh = CloneMesh(originalMesh, meshAttributes);
      clonedMesh.name = originalMesh.name;
      var meshList = new List<Mesh>();
      var meshWithBadTriangles = (Mesh)null;
      Mesh currentMesh;
      for (currentMesh = clonedMesh;
           DivideMeshOnBadAndGoodTriangles(currentMesh, out var badTriangles, out var goodTriangles);
           currentMesh = meshWithBadTriangles)
      {
        meshList.Add(BuildMeshFromTriangles(currentMesh, goodTriangles, false, meshAttributes));
        meshWithBadTriangles = BuildMeshFromTriangles(currentMesh, badTriangles, true, meshAttributes);
        if (currentMesh != null)
        {
          Object.DestroyImmediate(currentMesh);
          currentMesh = null;
        }

        var hasBadTriangles = false;
        if (meshWithBadTriangles.vertexCount < 3 || meshWithBadTriangles.triangles.Length < 1)
        {
          Debug.LogWarning("Mesh '" + originalMesh.name + "' has triangle(s) with wrong indices.\n");
          break;
        }

        var triangles = meshWithBadTriangles.triangles;
        for (var message = 0; message < triangles.Length; message += 3)
        {
          if (triangles[message] != triangles[message + 1] && triangles[message] != triangles[message + 2] &&
              triangles[message + 1] != triangles[message + 2]) continue;
          Debug.Log(message);
          hasBadTriangles = true;
          break;
        }

        if (!hasBadTriangles) continue;
        Debug.LogWarning($"Mesh '{originalMesh.name}' has triangle(s) with wrong indices.\n");
        break;
      }

      if (currentMesh != null) meshList.Add(currentMesh);

      var finalMesh = meshList[0];
      for (var index = 1; index < meshList.Count; ++index)
      {
        MeshMerger.MergeMeshes(ref finalMesh, meshList[index], originalMesh.name, meshAttributes);
        if (finalMesh == null) return null;
      }

      if (meshWithBadTriangles != null)
      {
        meshWithBadTriangles.Clear(false);
        Object.DestroyImmediate(meshWithBadTriangles);
      }

      for (var index = meshList.Count - 1; index >= 1; --index)
      {
        if (meshList[index] == null) continue;
        meshList[index].Clear(false);
        Object.DestroyImmediate(meshList[index]);
        meshList[index] = null;
      }

      return finalMesh;
    }

    

    private static Mesh GenerateFast(Mesh _origMesh, MeshAttributes meshAttributes)
    {
      if (_origMesh == null || _origMesh.vertexCount < 3 || _origMesh.triangles.Length == 0)
      {
        Debug.LogError("Can not generte wireframe from empty mesh.\n");
        return null;
      }

      if (_origMesh.triangles.Length > 63000)
      {
        Debug.LogWarning("Wire warning: " + _origMesh.name + " - Mesh triangle count is more than 21000.\n");
        return null;
      }

      meshAttributes ??= new MeshAttributes();
      var vertices = _origMesh.vertices;
      var normals = _origMesh.normals;
      var tangents = _origMesh.tangents;
      var uvs1 = new List<Vector4>();
      _origMesh.GetUVs(0, uvs1);
      var uvs2 = new List<Vector4>();
      if (meshAttributes.UseUV2)
        _origMesh.GetUVs(1, uvs2);
      var uvs3 = new List<Vector4>();
      if (meshAttributes.UseUV3)
        _origMesh.GetUVs(2, uvs3);
      var uvs4 = new List<Vector4>();
      if (meshAttributes.UseUV4)
        _origMesh.GetUVs(3, uvs4);
      var colors = _origMesh.colors;
      var boneWeights = _origMesh.boneWeights;
      var bindposes = _origMesh.bindposes;
      var flag1 = meshAttributes.UseNormals;
      if (flag1)
        flag1 = _origMesh.normals != null && _origMesh.normals.Length == _origMesh.vertexCount;
      var flag2 = meshAttributes.UseTangents;
      if (flag2)
        flag2 = _origMesh.tangents != null && _origMesh.tangents.Length == _origMesh.vertexCount;
      if (uvs1 == null || uvs1.Count != _origMesh.vertexCount)
      {
        uvs1 = new List<Vector4>(_origMesh.vertexCount);
        for (var index = 0; index < _origMesh.vertexCount; ++index)
          uvs1.Add(Vector4.zero);
      }

      var flag3 = meshAttributes.UseUV2;
      if (flag3)
        flag3 = _origMesh.uv2 != null && _origMesh.uv2.Length == _origMesh.vertexCount;
      var flag4 = meshAttributes.UseUV3;
      if (flag4)
        flag4 = _origMesh.uv3 != null && _origMesh.uv3.Length == _origMesh.vertexCount;
      var flag5 = meshAttributes.UseUV4;
      if (flag5)
        flag5 = _origMesh.uv4 != null && _origMesh.uv4.Length == _origMesh.vertexCount;
      var flag6 = meshAttributes.UseColors;
      if (flag6)
        flag6 = _origMesh.colors != null && _origMesh.colors.Length == _origMesh.vertexCount;
      var flag7 = meshAttributes.UseSkin;
      if (flag7)
        flag7 = _origMesh.boneWeights != null && _origMesh.boneWeights.Length == _origMesh.vertexCount;
      var fast = new Mesh
      {
        name = _origMesh.name,
        subMeshCount = _origMesh.subMeshCount
      };
      var length1 = _origMesh.triangles.Length;
      var vector3Array1 = new Vector3[length1];
      var numArrayList = new List<int[]>();
      var vector3Array2 = new Vector3[flag1 ? length1 : 0];
      var vector4Array = new Vector4[flag2 ? length1 : 0];
      var collection1 = new Vector4[length1];
      var collection2 = new Vector4[flag3 ? length1 : 0];
      var collection3 = new Vector4[flag4 ? length1 : 0];
      var collection4 = new Vector4[flag5 ? length1 : 0];
      var colorArray = new Color[flag6 ? length1 : 0];
      var boneWeightArray = new BoneWeight[flag7 ? length1 : 0];
      var barycentricArray = new BarycentricCoords[3]
      {
        new(1, 0, 0),
        new(0, 1, 0),
        new(0, 0, 1)
      };
      var num = 0;
      var index1 = -1;
      for (var submesh = 0; submesh < _origMesh.subMeshCount; ++submesh)
      {
        var length2 = _origMesh.GetTriangles(submesh).Length / 3 * 3;
        var numArray = new int[length2];
        var triangles = _origMesh.GetTriangles(submesh);
        for (var index2 = 0; index2 < length2; ++index2)
        {
          var index3 = triangles[index2];
          vector3Array1[++index1] = vertices[index3];
          numArray[index2] = num++;
          if (flag1)
            vector3Array2[index1] = normals[index3];
          if (flag2)
            vector4Array[index1] = tangents[index3];
          collection1[index1] = BarycentricCoordsHelper.BakeBarycentricToUV(uvs1[index3], barycentricArray[index2 % 3]);
          if (flag3)
            collection2[index1] = uvs2[index3];
          if (flag4)
            collection3[index1] = uvs3[index3];
          if (flag5)
            collection4[index1] = uvs4[index3];
          if (flag6)
            colorArray[index1] = colors[index3];
          if (flag7)
            boneWeightArray[index1] = boneWeights[index3];
        }

        numArrayList.Add(numArray);
      }

      fast.vertices = vector3Array1;
      fast.normals = vector3Array2;
      fast.tangents = vector4Array;
      fast.SetUVs(0, new List<Vector4>(collection1));
      if (flag3)
        fast.SetUVs(1, new List<Vector4>(collection2));
      if (flag4)
        fast.SetUVs(2, new List<Vector4>(collection3));
      if (flag5)
        fast.SetUVs(3, new List<Vector4>(collection4));
      fast.colors = colorArray;
      if (flag7)
      {
        fast.boneWeights = boneWeightArray;
        fast.bindposes = bindposes;
      }

      for (var index4 = 0; index4 < numArrayList.Count; ++index4)
        fast.SetTriangles(numArrayList[index4], index4);
      return fast;
    }

#endregion
  }
}