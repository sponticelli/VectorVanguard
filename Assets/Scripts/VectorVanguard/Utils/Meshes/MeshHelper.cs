using UnityEngine;

namespace VectorVanguard.Utils.Meshes
{
  public static class MeshHelper
  {
    /// <summary>
    /// Generates a custom mesh with a grid of triangles in a staggered arrangement, creating a mesh that consists of quads.
    /// </summary>
    /// <param name="numVerticesX">The number of vertices along the X-axis.</param>
    /// <param name="numVerticesY">The number of vertices along the Y-axis.</param>
    /// <param name="edgeLength">The length of each edge in the mesh</param>
    /// <returns>A Mesh object with the specified number of vertices and edge length.</returns>
    public static Mesh GenerateStaggeredGridMesh(int numVerticesX, int numVerticesY, float edgeLength = 5)
    {
      var vertices = new Vector3[numVerticesX * numVerticesY];
      var triangles = new int[(numVerticesX - 1) * (numVerticesY - 1) * 6];
      var triIndex = 0;
      var stepY = 1f / (numVerticesY - 1f);
      var stepX = stepY * Mathf.Sqrt(0.75f);
      var halfSize = new Vector2((numVerticesX - 1) * stepX, (numVerticesY - 1) * stepY - stepY * 0.5f) * 0.5f;
      for (var y = 0; y < numVerticesY; y++)
      {
        for (var x = 0; x < numVerticesX; x++)
        {
          var i = y * numVerticesX + x;
          
          if (x % 2 == 0)
          {
            vertices[i] = (new Vector2(x * stepX, y * stepY) - halfSize) * ((edgeLength + stepY * 6) * 2);
            if (x == numVerticesX - 1 || y == numVerticesY - 1) continue;
            triangles[triIndex++] = i + numVerticesX + 1;
            triangles[triIndex++] = i;
            triangles[triIndex++] = i + numVerticesX;

            triangles[triIndex++] = i;
            triangles[triIndex++] = i + numVerticesX + 1;
            triangles[triIndex++] = i + 1;
          }
          else
          {
            vertices[i] = (new Vector2(x * stepX, (y - 0.5f) * stepY) - halfSize) * ((edgeLength + stepY * 6) * 2);
            if (x == numVerticesX - 1 || y == numVerticesY - 1) continue;
            triangles[triIndex++] = i + numVerticesX;
            triangles[triIndex++] = i + numVerticesX + 1;
            triangles[triIndex++] = i + 1;

            triangles[triIndex++] = i + 1;
            triangles[triIndex++] = i;
            triangles[triIndex++] = i + numVerticesX;
          }
        }
      }

      //Debug.Log("verts : " + vertices.Length + "     triangles: " + (triangles.Length / 3));
      return CreateMesh(vertices, triangles);
    }


    /// <summary>
    /// Creates a Mesh object using the provided vertex positions and triangle indices.
    /// </summary>
    /// <param name="vertices">An array of Vector3 objects representing the vertex positions.</param>
    /// <param name="triangles">An array of integers representing the triangle indices.</param>
    /// <returns>A Mesh object with the specified vertices and triangles.</returns>
    public static Mesh CreateMesh(Vector3[] vertices, int[] triangles)
    {
      var mesh = new Mesh
      {
        vertices = vertices,
        triangles = triangles
      };
      mesh.RecalculateNormals();
      return mesh;
    }
  }
}