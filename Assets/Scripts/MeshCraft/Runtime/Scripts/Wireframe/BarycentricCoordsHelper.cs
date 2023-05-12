using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MeshCraft.Wireframe
{
  public static class BarycentricCoordsHelper
  {
    private static readonly BarycentricCoords ZeroCoord = new(0, 0, 0);

    private static readonly BarycentricCoords[] UnitCoords =
    {
      new(1, 0, 0),
      new(0, 1, 0),
      new(0, 0, 1)
    };


    /// <summary>
    /// Generates an array of BarycentricCoords values for each vertex of a mesh based on its triangle indices.
    /// </summary>
    /// <param name="vertexCount">The total number of vertices in the mesh.</param>
    /// <param name="triangles">An array of indices representing the triangles of the mesh.</param>
    /// <returns>An array of BarycentricCoords values, where each element corresponds to a vertex of the mesh.</returns>
    public static BarycentricCoords[] GenerateBarycentricCoords(int vertexCount, ref int[] triangles)
    {
      var barycentricCoords = InitializeBarycentricCoordsArray(vertexCount);

      for (var i = 0; i < triangles.Length; i += 3)
      {
        var vertexIndices = new[] { triangles[i], triangles[i + 1], triangles[i + 2] };
        var anyAssigned = CheckIfAnyAssigned(barycentricCoords, vertexIndices);

        AssignBarycentricCoordsToTriangleVertices(barycentricCoords, vertexIndices, UnitCoords, anyAssigned);
      }

      return barycentricCoords;
    }


    public static void BakeBarycentricToUV(IList<Vector4> originalUV,
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
    public static Vector4 BakeBarycentricToUV(Vector4 originalUV, BarycentricCoords barycentricCoords)
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


    public static BarycentricCoords UVToBarycentric(Vector4 uv)
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
    /// Initializes an array of BarycentricCoords of the specified size with zero values.
    /// </summary>
    /// <param name="vertexCount">The size of the array to be created.</param>
    /// <returns>A new array of BarycentricCoords with the specified size, initialized with zero values.</returns>
    private static BarycentricCoords[] InitializeBarycentricCoordsArray(int vertexCount)
    {
      var barycentricCoords = new BarycentricCoords[vertexCount];
      for (var i = 0; i < vertexCount; ++i)
      {
        barycentricCoords[i] = ZeroCoord;
      }

      return barycentricCoords;
    }

    /// <summary>
    /// Checks if any vertex in the given list of indices has already been assigned a non-zero Barycentric coordinate.
    /// </summary>
    /// <param name="barycentricCoords">The list of Barycentric coordinates.</param>
    /// <param name="vertexIndices">The list of vertex indices to check.</param>
    /// <returns>True if any vertex in the list of indices has a non-zero Barycentric coordinate, false otherwise.</returns>
    private static bool CheckIfAnyAssigned(IReadOnlyList<BarycentricCoords> barycentricCoords, IReadOnlyList<int> vertexIndices)
    {
      return barycentricCoords[vertexIndices[0]] != ZeroCoord
             || barycentricCoords[vertexIndices[1]] != ZeroCoord
             || barycentricCoords[vertexIndices[2]] != ZeroCoord;
    }

    /// <summary>
    /// Assigns Barycentric coordinates to the vertices of a triangle based on their indices.
    /// </summary>
    /// <param name="barycentricCoords">The list of Barycentric coordinates to be modified.</param>
    /// <param name="vertexIndices">The list of vertex indices for the triangle.</param>
    /// <param name="unitCoords">The list of unit Barycentric coordinates to be assigned.</param>
    /// <param name="anyAssigned">Indicates if any vertex in the current triangle has already been assigned a
    /// non-zero Barycentric coordinate.</param>
    private static void AssignBarycentricCoordsToTriangleVertices(IList<BarycentricCoords> barycentricCoords,
      IReadOnlyList<int> vertexIndices, IReadOnlyList<BarycentricCoords> unitCoords, bool anyAssigned)
    {
      for (var i = 0; i < 3; i++)
      {
        var current = vertexIndices[i];
        var next1 = vertexIndices[(i + 1) % 3];
        var next2 = vertexIndices[(i + 2) % 3];

        if (barycentricCoords[current] == ZeroCoord && !anyAssigned)
        {
          barycentricCoords[current] = unitCoords[i];
        }
        else if (barycentricCoords[current] != ZeroCoord)
        {
          if (barycentricCoords[next1] == ZeroCoord)
          {
            barycentricCoords[next1] = barycentricCoords[next2] != unitCoords[(i + 2) % 3]
              ? unitCoords[(i + 2) % 3]
              : unitCoords[(i + 1) % 3];
          }

          if (barycentricCoords[next2] == ZeroCoord)
          {
            barycentricCoords[next2] = barycentricCoords[next1] != unitCoords[(i + 1) % 3]
              ? unitCoords[(i + 1) % 3]
              : unitCoords[(i + 2) % 3];
          }
        }
      }
    }
  }
}