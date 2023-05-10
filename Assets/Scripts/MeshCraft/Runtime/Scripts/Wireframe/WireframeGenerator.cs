using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LiteNinja.MeshCraft
{
  public class WireframeGenerator
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

    public static Mesh GenerateQuads(Mesh originalMesh, float normalCoef = 1f, float angleCoef = 1f, float areaCoef = 1f)
    {
      return GenerateQuads(originalMesh, normalCoef, angleCoef, areaCoef, new MeshAttributes());
    }

    public static Mesh GenerateQuads(Mesh originalMesh, float normalCoef, float angleCoef, float areaCoef, bool uv2,
      bool uv3, bool uv4, bool normal, bool tangent, bool color, bool skin)
    {
      return GenerateQuads(originalMesh, normalCoef, angleCoef, areaCoef, new MeshAttributes
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
    /// Generates an array of BarycentricCoords values for each vertex of a mesh based on its triangle indices.
    /// </summary>
    /// <param name="vertexCount">The total number of vertices in the mesh.</param>
    /// <param name="triangles">An array of indices representing the triangles of the mesh.</param>
    /// <returns>An array of BarycentricCoords values, where each element corresponds to a vertex of the mesh.</returns>
    private static BarycentricCoords[] GenerateBarycentricCoords(int vertexCount, ref int[] triangles)
    {
      var xOne = new BarycentricCoords(1, 0, 0);
      var yOne = new BarycentricCoords(0, 1, 0);
      var zOne = new BarycentricCoords(0, 0, 1);
      var zero = new BarycentricCoords(0, 0, 0);
      var barycentricMass = new BarycentricCoords[vertexCount];

      for (var index = 0; index < vertexCount; ++index)
      {
        barycentricMass[index] = zero;
      }

      for (var index1 = 0; index1 < triangles.Length; index1 += 3)
      {
        var index2 = triangles[index1];
        var index3 = triangles[index1 + 1];
        var index4 = triangles[index1 + 2];

        // Check if any vertex has been assigned a Barycentric value
        var anyAssigned = barycentricMass[index2] != zero || barycentricMass[index3] != zero || barycentricMass[index4] != zero;

        var choices = new[] { xOne, yOne, zOne };

        for (var i = 0; i < 3; i++)
        {
          var indices = new[] { index2, index3, index4 };
          var current = indices[i];
          var next1 = indices[(i + 1) % 3];
          var next2 = indices[(i + 2) % 3];

          if (barycentricMass[current] == zero && !anyAssigned)
          {
            barycentricMass[current] = choices[i];
          }
          else if (barycentricMass[current] != zero)
          {
            if (barycentricMass[next1] == zero)
            {
              barycentricMass[next1] = barycentricMass[next2] != choices[(i + 2) % 3] ? choices[(i + 2) % 3] : choices[(i + 1) % 3];
            }
            if (barycentricMass[next2] == zero)
            {
              barycentricMass[next2] = barycentricMass[next1] != choices[(i + 1) % 3] ? choices[(i + 1) % 3] : choices[(i + 2) % 3];
            }
          }
        }
      }

      return barycentricMass;
    }


    private static void BakeBarycentricToUV(IList<Vector4> originalUV,
      IReadOnlyList<BarycentricCoords> barycentricCoords)
    {
      if (originalUV == null || barycentricCoords == null || originalUV.Count != barycentricCoords.Count)
      {
        Debug.LogError("BakeBarycentricToUV() problems.\n");
      }
      else
      {
        var count = originalUV.Count;
        for (var index = 0; index < count; ++index)
        {
          originalUV[index] = BakeBarycentricToUV(originalUV[index], barycentricCoords[index]);
        }
      }
    }
    
    /// <summary>
    /// Combines the original UV coordinates with the Barycentric coordinates to create a new Vector4 value.
    /// </summary>
    private static Vector4 BakeBarycentricToUV(Vector4 originalUV, BarycentricCoords barycentricCoords)
    {
      // Compute the 'z' value based on the Barycentric coordinates x and y.
      // If x is not 1, and y is not 1, then z is 0.0f.
      // If x is not 1, and y is 1, then z is 0.1f.
      // If x is 1, and y is not 1, then z is 1.0f.
      // If x is 1, and y is 1, then z is 1.1f.
      var z = barycentricCoords.x != 1
        ? (barycentricCoords.y != 1 ? 0.0f : 0.1f)
        : (barycentricCoords.y != 1 ? 1f : 1.1f);

      // Create a new Vector4 using the originalUV x and y components, the computed 'z' value, and the Barycentric coordinate 'z'.
      // The resulting Vector4 combines the original UV coordinates with the Barycentric coordinates
      return new Vector4(originalUV.x, originalUV.y, z, barycentricCoords.z);
    }
    
    
    private static BarycentricCoords UVToBarycentric(Vector4 uv)
    {
      // If the 'z' component of the input UV vector is not greater than 0.001,
      // return a new BarycentricCoords object with x = 0, y = 0, and z equal to the integer part of uv.w.
      if (uv.z <= 0.001) return new BarycentricCoords(0, 0, (int)uv.w);

      // Set x and y values based on the 'z' component of the input UV vector.
      var x = uv.z > 0.5 ? 1 : 0;
      var y = uv.z > 0.5 ? 0 : 1;

      // Return a new BarycentricCoords object with the computed x, y, and z equal to the integer part of uv.w.
      return new BarycentricCoords(x, y, (int)uv.w);
    }

    /// <summary>
    /// a good triangle is a triangle whose vertices have Barycentric coordinates that sum up to 1 for each of the
    /// x, y, and z components. This means that the Barycentric coordinates of the vertices in a good triangle form a
    /// valid barycentric representation
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="badTriangles"></param>
    /// <param name="goodTriangles"></param>
    /// <returns></returns>
    private static bool DivideMeshOnBadAndGoodTriangles(Mesh mesh, out List<int[]> badTriangles, out List<int[]> goodTriangles)
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
          var barycentric1 = UVToBarycentric(uvs[triangles[index]]);
          var barycentric2 = UVToBarycentric(uvs[triangles[index + 1]]);
          var barycentric3 = UVToBarycentric(uvs[triangles[index + 2]]);
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
    private static Mesh BuildMeshFromTriangles(Mesh origMesh, List<int[]> trianglesIndices, bool rebuildBarycentricCoords,
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
        var barycentricMass = GenerateBarycentricCoords(mesh.vertexCount, ref triangles);
        BakeBarycentricToUV(uv1ForNewMesh, barycentricMass);
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
      var barycentricCoords = GenerateBarycentricCoords(mesh.vertexCount, ref triangles);
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

      BakeBarycentricToUV(uv1List, barycentricCoords);
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
    /// Clones a mesh with flattened triangles and selectively preserves its attributes based on the provided MeshAttributes object.
    /// </summary>
    /// <param name="originalMesh">The original mesh to clone and flatten.</param>
    /// <param name="meshAttributes">An object specifying which mesh attributes (normals, tangents, UVs, etc.) to preserve in the cloned and flattened mesh.</param>
    /// <returns>A new mesh that is a clone of the original mesh with flattened triangles and selected mesh attributes preserved.</returns>
    private static Mesh CloneMeshAsFlat(Mesh originalMesh, MeshAttributes meshAttributes)
    {
      var vertices = originalMesh.vertices;
      var normals = originalMesh.normals;
      var tangents = originalMesh.tangents;
      var uvs1 = new List<Vector4>();
      originalMesh.GetUVs(0, uvs1);
      var uvs2 = new List<Vector4>();
      originalMesh.GetUVs(1, uvs2);
      var uvs3 = new List<Vector4>();
      originalMesh.GetUVs(2, uvs3);
      var uvs4 = new List<Vector4>();
      originalMesh.GetUVs(3, uvs4);
      var colors = originalMesh.colors;
      var boneWeights = originalMesh.boneWeights;
      var vertexList = new List<Vector3>();
      var triangleIndexGroups = new List<List<int>>();
      var uv1Coordinates = new List<Vector4>();
      var uv2Coordinates = new List<Vector4>();
      var uv3Coordinates = new List<Vector4>();
      var uv4Coordinates = new List<Vector4>();
      var normalList = new List<Vector3>();
      var tangentsList = new List<Vector4>();
      var colorList = new List<Color>();
      var boneWeightList = new List<BoneWeight>();
      var useUV2 = true;
      var useUV3 = true;
      var useUV4 = true;
      var useNormals = true;
      var useTangents = true;
      var useColors = true;
      var useSkin = true;
      if (originalMesh.uv == null || originalMesh.uv.Length != originalMesh.vertexCount)
      {
        uvs1 = new List<Vector4>();
        for (var index = 0; index < originalMesh.vertexCount; ++index)
        {
          uvs1.Add(Vector4.zero);
        }
      }

      if (originalMesh.uv2 == null || originalMesh.uv2.Length != originalMesh.vertexCount || !meshAttributes.UseUV2) useUV2 = false;
      if (originalMesh.uv3 == null || originalMesh.uv3.Length != originalMesh.vertexCount || !meshAttributes.UseUV3) useUV3 = false;
      if (originalMesh.uv4 == null || originalMesh.uv4.Length != originalMesh.vertexCount || !meshAttributes.UseUV4) useUV4 = false;
      if (originalMesh.normals == null || originalMesh.normals.Length != originalMesh.vertexCount || !meshAttributes.UseNormals) useNormals = false;
      if (originalMesh.tangents == null || originalMesh.tangents.Length != originalMesh.vertexCount || !meshAttributes.UseTangents) useTangents = false;
      if (originalMesh.colors == null || originalMesh.colors.Length != originalMesh.vertexCount || !meshAttributes.UseColors) useColors = false;
      if (originalMesh.boneWeights == null || originalMesh.boneWeights.Length != originalMesh.vertexCount || !meshAttributes.UseSkin) useSkin = false;
      var vertexIndex = 0;
      for (var subMesh = 0; subMesh < originalMesh.subMeshCount; ++subMesh)
      {
        var triangles = originalMesh.GetTriangles(subMesh);
        triangleIndexGroups.Add(new List<int>());
        for (var index1 = 0; index1 < triangles.Length; index1 += 3)
        {
          var index2 = triangles[index1];
          var index3 = triangles[index1 + 1];
          var index4 = triangles[index1 + 2];
          var firstSubMeshTriangleIndices = triangleIndexGroups[^1];
          var firstVertexIndex = vertexIndex;
          var secondVertexIndex = firstVertexIndex + 1;
          firstSubMeshTriangleIndices.Add(firstVertexIndex);
          var triangleIndexList = triangleIndexGroups[^1];
          var thirdVertexIndex = secondVertexIndex + 1;
          triangleIndexList.Add(secondVertexIndex);
          var triangleIndices = triangleIndexGroups[^1];
          vertexIndex = thirdVertexIndex + 1;
          triangleIndices.Add(thirdVertexIndex);
          vertexList.Add(vertices[index2]);
          vertexList.Add(vertices[index3]);
          vertexList.Add(vertices[index4]);
          uv1Coordinates.Add(uvs1[index2]);
          uv1Coordinates.Add(uvs1[index3]);
          uv1Coordinates.Add(uvs1[index4]);
          if (useUV2)
          {
            uv2Coordinates.Add(uvs2[index2]);
            uv2Coordinates.Add(uvs2[index3]);
            uv2Coordinates.Add(uvs2[index4]);
          }

          if (useUV3)
          {
            uv3Coordinates.Add(uvs3[index2]);
            uv3Coordinates.Add(uvs3[index3]);
            uv3Coordinates.Add(uvs3[index4]);
          }

          if (useUV4)
          {
            uv4Coordinates.Add(uvs4[index2]);
            uv4Coordinates.Add(uvs4[index3]);
            uv4Coordinates.Add(uvs4[index4]);
          }

          if (useNormals)
          {
            normalList.Add(normals[index2]);
            normalList.Add(normals[index3]);
            normalList.Add(normals[index4]);
          }

          if (useTangents)
          {
            tangentsList.Add(tangents[index2]);
            tangentsList.Add(tangents[index3]);
            tangentsList.Add(tangents[index4]);
          }

          if (useColors)
          {
            colorList.Add(colors[index2]);
            colorList.Add(colors[index3]);
            colorList.Add(colors[index4]);
          }

          if (useSkin)
          {
            boneWeightList.Add(boneWeights[index2]);
            boneWeightList.Add(boneWeights[index3]);
            boneWeightList.Add(boneWeights[index4]);
          }
        }
      }

      var newMesh = new Mesh
      {
        subMeshCount = originalMesh.subMeshCount,
        vertices = vertexList.ToArray()
      };
      for (var index = 0; index < triangleIndexGroups.Count; ++index)
      {
        newMesh.SetTriangles(triangleIndexGroups[index].ToArray(), index);
      }
      newMesh.SetUVs(0, new List<Vector4>(uv1Coordinates));
      if (useUV2) newMesh.SetUVs(1, new List<Vector4>(uv2Coordinates));
      if (useUV3) newMesh.SetUVs(2, new List<Vector4>(uv3Coordinates));
      if (useUV4) newMesh.SetUVs(3, new List<Vector4>(uv4Coordinates));
      if (useNormals) newMesh.normals = normalList.ToArray();
      if (useTangents) newMesh.tangents = tangentsList.ToArray();
      if (useColors) newMesh.colors = colorList.ToArray();
      if (useSkin)
      {
        newMesh.boneWeights = boneWeightList.ToArray();
        newMesh.bindposes = originalMesh.bindposes;
      }

      return newMesh;
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
      for (currentMesh = clonedMesh; DivideMeshOnBadAndGoodTriangles(currentMesh, out var badTriangles, out var goodTriangles); currentMesh = meshWithBadTriangles)
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

    /// <summary>
    /// Generates a mesh of quads based on an input mesh, using specified coefficients to control the generation process.
    /// </summary>
    /// <param name="originalMesh">The original mesh to generate quads from.</param>
    /// <param name="normalCoef">The coefficient for normal difference.</param>
    /// <param name="angleCoef">The coefficient for angle difference.</param>
    /// <param name="areaCoef">The coefficient for area difference.</param>
    /// <param name="meshAttributes">The mesh attributes to use for the generated quads.</param>
    /// <returns>A new mesh containing the generated quads, or null if the generation failed.</returns>
    private static Mesh GenerateQuads(Mesh originalMesh, float normalCoef, float angleCoef, float areaCoef, MeshAttributes meshAttributes)
    {
      if (originalMesh == null)
      {
        Debug.LogWarning("WireframeGenerator.GenerateQuads: 'originalMesh' == null\n");
        return null;
      }

      if (originalMesh.triangles.Length / 3 > 21000)
      {
        Debug.LogWarning($"WireframeGenerator.GenerateQuads: '{originalMesh.name}' mesh vertex count can not be more than 21000\n");
        return null;
      }

      var quads = CloneMeshAsFlat(originalMesh, meshAttributes);
      quads.name = originalMesh.name;
      Triangle.NormalCoefficient = normalCoef;
      Triangle.AngleCoefficient = angleCoef;
      Triangle.AreaCoefficient = areaCoef;
      var vertices = quads.vertices;
      var triangles = quads.triangles;
      var triangleArray = new Triangle[triangles.Length / 3];
      var dictionary = new Dictionary<Edge, List<int>>();
      for (var index = 0; index < triangles.Length; index += 3)
      {
        var _index = index / 3;
        var vertex1 = new Vertex(vertices[triangles[index]]);
        var vertex2 = new Vertex(vertices[triangles[index + 1]]);
        var vertex3 = new Vertex(vertices[triangles[index + 2]]);
        var edge1 = new Edge(vertex1, vertex2);
        var edge2 = new Edge(vertex2, vertex3);
        var edge3 = new Edge(vertex3, vertex1);
        if (!dictionary.ContainsKey(edge1))
        {
          dictionary.Add(edge1, new List<int> { _index });
        }
        else
        {
          dictionary[edge1].Add(_index);
        }

        if (!dictionary.ContainsKey(edge2))
        {
          dictionary.Add(edge2, new List<int> { _index });
        }
        else
        {
          dictionary[edge2].Add(_index);
        }

        if (!dictionary.ContainsKey(edge3))
        {
          dictionary.Add(edge3, new List<int> { _index });
        }
        else
        {
          dictionary[edge3].Add(_index);
        }
        
        var normalized = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;
        triangleArray[_index] = new Triangle(_index, normalized, edge1, edge2, edge3);
      }

      foreach (var intList in dictionary.Select(keyValuePair => keyValuePair.Value))
      {
        for (var index1 = 0; index1 < intList.Count; ++index1)
        {
          for (var index2 = 0; index2 < intList.Count; ++index2)
          {
            if (index1 == index2) continue;
            var triangle = triangleArray[intList[index1]];
            var key = triangleArray[intList[index2]];
            triangle.edge0.GetHashCode();
            var hashCode1 = triangle.edge1.GetHashCode();
            var hashCode2 = triangle.edge2.GetHashCode();
            var hashCode3 = key.edge0.GetHashCode();
            var hashCode4 = key.edge1.GetHashCode();
            var hashCode5 = key.edge2.GetHashCode();
            var _index = 0;
            if (hashCode1 == hashCode3 || hashCode1 == hashCode4 || hashCode1 == hashCode5)
            {
              _index = 1;
            }
            else if (hashCode2 == hashCode3 || hashCode2 == hashCode4 || hashCode2 == hashCode5)
            {
              _index = 2;
            }

            var hashCode6 = triangle.edge0.v1.GetHashCode();
            var hashCode7 = triangle.edge1.v1.GetHashCode();
            var hashCode8 = triangle.edge2.v1.GetHashCode();
            var hashCode9 = key.edge0.v1.GetHashCode();
            var hashCode10 = key.edge1.v1.GetHashCode();
            var hashCode11 = key.edge2.v1.GetHashCode();
            var _0To = -1;
            if (hashCode6 == hashCode9)
              _0To = 0;
            else if (hashCode6 == hashCode10)
              _0To = 1;
            else if (hashCode6 == hashCode11)
              _0To = 2;
            var _1To = -1;
            if (hashCode7 == hashCode9)
              _1To = 0;
            else if (hashCode7 == hashCode10)
              _1To = 1;
            else if (hashCode7 == hashCode11)
              _1To = 2;
            var _2To = -1;
            if (hashCode8 == hashCode9)
              _2To = 0;
            else if (hashCode8 == hashCode10)
              _2To = 1;
            else if (hashCode8 == hashCode11)
              _2To = 2;
            if (!triangle.neighbors.ContainsKey(key))
              triangle.neighbors.Add(key, new NeighborInfo(_index, _0To, _1To, _2To));
          }
        }
      }

      foreach (var t in triangleArray)
      {
        t.GenerateNeighborInfo();
      }

      foreach (var t in triangleArray)
      {
        t.FindBestMatch();
      }

      var uvs = new List<Vector4>();
      quads.GetUVs(0, uvs);
      var collection = new Vector4[quads.vertexCount];
      foreach (var t in triangleArray)
      {
        var index4 = t.Index * 3;
        collection[index4] = BakeBarycentricToUV(uvs[index4], t.barycentricCoords0);
        collection[index4 + 1] = BakeBarycentricToUV(uvs[index4 + 1], t.barycentricCoords1);
        collection[index4 + 2] = BakeBarycentricToUV(uvs[index4 + 2], t.barycentricCoords2);
      }

      quads.SetUVs(0, new List<Vector4>(collection));
      Random.InitState(0);
      return quads;
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

      if (meshAttributes == null)
        meshAttributes = new MeshAttributes();
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
          collection1[index1] = BakeBarycentricToUV(uvs1[index3], barycentricArray[index2 % 3]);
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