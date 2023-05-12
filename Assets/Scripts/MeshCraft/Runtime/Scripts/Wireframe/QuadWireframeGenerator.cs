using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LiteNinja.MeshCraft.Wireframe
{
  public static class QuadWireframeGenerator
  {
    public static Mesh Generate(Mesh originalMesh, float normalCoefficient = 1f, float angleCoefficient = 1f,
      float areaCoefficient = 1f)
    {
      return Generate(originalMesh, normalCoefficient, angleCoefficient, areaCoefficient, new MeshAttributes());
    }
    
    public static Mesh Generate(Mesh originalMesh, float normalCoefficient, float angleCoefficient,
      float areaCoefficient, bool uv2,
      bool uv3, bool uv4, bool normal, bool tangent, bool color, bool skin)
    {
      return Generate(originalMesh, normalCoefficient, angleCoefficient, areaCoefficient, new MeshAttributes
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

    /// <summary>
    /// Generates a mesh of quads based on an input mesh, using specified coefficients to control the generation process.
    /// </summary>
    /// <param name="originalMesh">The original mesh to generate quads from.</param>
    /// <param name="normalCoefficient">The coefficient for normal difference.</param>
    /// <param name="angleCoefficient">The coefficient for angle difference.</param>
    /// <param name="areaCoefficient">The coefficient for area difference.</param>
    /// <param name="meshAttributes">The mesh attributes to use for the generated quads.</param>
    /// <returns>A new mesh containing the generated quads, or null if the generation failed.</returns>
    private static Mesh Generate(Mesh originalMesh, float normalCoefficient, float angleCoefficient,
      float areaCoefficient, MeshAttributes meshAttributes)
    {
      if (originalMesh == null)
      {
        Debug.LogWarning("QuadWireframeGenerator.Generate: 'originalMesh' == null\n");
        return null;
      }

      if (originalMesh.triangles.Length / 3 > 21000)
      {
        Debug.LogWarning(
          $"QuadWireframeGenerator.Generate: '{originalMesh.name}' mesh vertex count can not be more than 21000\n");
        return null;
      }

      Triangle.NormalCoefficient = normalCoefficient;
      Triangle.AngleCoefficient = angleCoefficient;
      Triangle.AreaCoefficient = areaCoefficient;
      var quads = InitializeQuads(originalMesh, meshAttributes);
      var vertices = quads.vertices;
      var triangles = quads.triangles;
      var triangleArray = new Triangle[triangles.Length / 3];
      var edgeToTriangleIndices = new Dictionary<Edge, List<int>>();
      CreateTriangleArray(triangles, vertices, edgeToTriangleIndices, triangleArray);
      ProcessNeighbors(edgeToTriangleIndices, triangleArray);

      foreach (var t in triangleArray) t.GenerateNeighborInfo();
      foreach (var t in triangleArray) t.FindBestMatch();
      UpdateUVsForQuads(quads, triangleArray);

      Random.InitState(0);
      return quads;
    }


    private static void UpdateUVsForQuads(Mesh quads, IEnumerable<Triangle> triangleArray)
    {
      var uvs = new List<Vector4>();
      quads.GetUVs(0, uvs);
      var collection = new Vector4[quads.vertexCount];
      foreach (var t in triangleArray)
      {
        var index4 = t.Index * 3;
        collection[index4] = BarycentricCoordsHelper.BakeBarycentricToUV(uvs[index4], t.barycentricCoords0);
        collection[index4 + 1] = BarycentricCoordsHelper.BakeBarycentricToUV(uvs[index4 + 1], t.barycentricCoords1);
        collection[index4 + 2] = BarycentricCoordsHelper.BakeBarycentricToUV(uvs[index4 + 2], t.barycentricCoords2);
      }

      quads.SetUVs(0, new List<Vector4>(collection));
    }

    private static void ProcessNeighbors(Dictionary<Edge, List<int>> edgeToTriangleIndices,
      IReadOnlyList<Triangle> triangleArray)
    {
      // Iterate through all the lists of triangle indices associated with each edge
      foreach (var triangleIndices in edgeToTriangleIndices.Values)
      {
        for (var index1 = 0; index1 < triangleIndices.Count; ++index1)
        {
          for (var index2 = 0; index2 < triangleIndices.Count; ++index2)
          {
            if (index1 == index2)
            {
              continue;
            }

            var triangle1 = triangleArray[triangleIndices[index1]];
            var triangle2 = triangleArray[triangleIndices[index2]];

            var sharedEdgeIndex = GetSharedEdgeIndex(triangle1, triangle2);
            var sharedVerticesIndices = GetSharedVerticesIndices(triangle1, triangle2);

            if (!triangle1.neighbors.ContainsKey(triangle2))
            {
              triangle1.neighbors.Add(triangle2,
                new TriangleNeighborRelationship(sharedEdgeIndex, sharedVerticesIndices[0], sharedVerticesIndices[1],
                  sharedVerticesIndices[2]));
            }
          }
        }
      }
    }

    private static int GetSharedEdgeIndex(Triangle triangle1, Triangle triangle2)
    {
      var triangle1EdgeHashCodes = new[]
        { triangle1.edge0.GetHashCode(), triangle1.edge1.GetHashCode(), triangle1.edge2.GetHashCode() };
      var triangle2EdgeHashCodes = new[]
        { triangle2.edge0.GetHashCode(), triangle2.edge1.GetHashCode(), triangle2.edge2.GetHashCode() };

      for (var i = 0; i < 3; i++)
      {
        if (triangle1EdgeHashCodes[i] == triangle2EdgeHashCodes[0] ||
            triangle1EdgeHashCodes[i] == triangle2EdgeHashCodes[1] ||
            triangle1EdgeHashCodes[i] == triangle2EdgeHashCodes[2])
        {
          return i;
        }
      }

      return -1;
    }

    private static int[] GetSharedVerticesIndices(Triangle triangle1, Triangle triangle2)
    {
      var triangle1VertexHashCodes = new[]
        { triangle1.edge0.v1.GetHashCode(), triangle1.edge1.v1.GetHashCode(), triangle1.edge2.v1.GetHashCode() };
      var triangle2VertexHashCodes = new[]
        { triangle2.edge0.v1.GetHashCode(), triangle2.edge1.v1.GetHashCode(), triangle2.edge2.v1.GetHashCode() };

      var sharedVerticesIndices = new int[3];

      for (var i = 0; i < 3; i++)
      {
        sharedVerticesIndices[i] = Array.IndexOf(triangle2VertexHashCodes, triangle1VertexHashCodes[i]);
      }

      return sharedVerticesIndices;
    }

    private static void CreateTriangleArray(IReadOnlyList<int> triangles, IReadOnlyList<Vector3> vertices,
      IDictionary<Edge, List<int>> edgeToTriangleIndices,
      IList<Triangle> triangleArray)
    {
      for (var index = 0; index < triangles.Count; index += 3)
      {
        var triangleIndex = index / 3;
        var vertex1 = new Vertex(vertices[triangles[index]]);
        var vertex2 = new Vertex(vertices[triangles[index + 1]]);
        var vertex3 = new Vertex(vertices[triangles[index + 2]]);
        var edge1 = new Edge(vertex1, vertex2);
        var edge2 = new Edge(vertex2, vertex3);
        var edge3 = new Edge(vertex3, vertex1);
        if (!edgeToTriangleIndices.ContainsKey(edge1))
        {
          edgeToTriangleIndices.Add(edge1, new List<int> { triangleIndex });
        }
        else
        {
          edgeToTriangleIndices[edge1].Add(triangleIndex);
        }

        if (!edgeToTriangleIndices.ContainsKey(edge2))
        {
          edgeToTriangleIndices.Add(edge2, new List<int> { triangleIndex });
        }
        else
        {
          edgeToTriangleIndices[edge2].Add(triangleIndex);
        }

        if (!edgeToTriangleIndices.ContainsKey(edge3))
        {
          edgeToTriangleIndices.Add(edge3, new List<int> { triangleIndex });
        }
        else
        {
          edgeToTriangleIndices[edge3].Add(triangleIndex);
        }

        var normalized = Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;
        triangleArray[triangleIndex] = new Triangle(triangleIndex, normalized, edge1, edge2, edge3);
      }
    }

    private static Mesh InitializeQuads(Mesh originalMesh, MeshAttributes meshAttributes)
    {
      var quads = CloneMeshAsFlat(originalMesh, meshAttributes);
      quads.name = originalMesh.name;
      return quads;
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

      if (originalMesh.uv2 == null || originalMesh.uv2.Length != originalMesh.vertexCount ||
          !meshAttributes.UseUV2) useUV2 = false;
      if (originalMesh.uv3 == null || originalMesh.uv3.Length != originalMesh.vertexCount ||
          !meshAttributes.UseUV3) useUV3 = false;
      if (originalMesh.uv4 == null || originalMesh.uv4.Length != originalMesh.vertexCount ||
          !meshAttributes.UseUV4) useUV4 = false;
      if (originalMesh.normals == null || originalMesh.normals.Length != originalMesh.vertexCount ||
          !meshAttributes.UseNormals) useNormals = false;
      if (originalMesh.tangents == null || originalMesh.tangents.Length != originalMesh.vertexCount ||
          !meshAttributes.UseTangents) useTangents = false;
      if (originalMesh.colors == null || originalMesh.colors.Length != originalMesh.vertexCount ||
          !meshAttributes.UseColors) useColors = false;
      if (originalMesh.boneWeights == null || originalMesh.boneWeights.Length != originalMesh.vertexCount ||
          !meshAttributes.UseSkin) useSkin = false;
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
  }
}