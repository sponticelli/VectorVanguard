using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  /// <summary>
  /// Generates a mesh for a path with a constant width
  /// </summary>
  public class CenteredPathMeshBuilder : IMeshBuilder
  {
    
    public float Width { get; set; } = 1f;
    public float Spacing { get; set; } = 0.01f;
    
    public CenteredPathMeshBuilder(float spacing,  float width)
    {
      Width = width;
      Spacing = spacing;
    }
    
    public Mesh BuildMesh(Path2D path)
    {
      if (Spacing <= 0)
      {
        Debug.LogError("Spacing must be greater than 0");
        return null;
      }
      
      if (path == null)
      {
        Debug.LogError("Path cannot be null");
        return null;
      }
      
      var points = path.GetEquidistantNodes(Spacing);
      var vertices = new Vector3[points.Length * 2];
      var numTriangles = ((points.Length-1) * 2) + ((path.IsClosed) ? 2 : 0);
      var triangles = new int[numTriangles * 3];
      var UVs = new Vector2[vertices.Length];
      var vertIndex = 0;
      var triIndex = 0;
      
      for (var i = 0; i < points.Length; i++)
      {
        var forward = Vector2.zero;
        if (i < points.Length - 1 || path.IsClosed)
        {
          forward += points[(i + 1) % points.Length] - points[i];
        }
        if (i > 0 || path.IsClosed)
        {
          forward += points[i] - points[(i - 1 + points.Length) % points.Length];
        }
        forward.Normalize();
        var left = new Vector2(-forward.y, forward.x);

        vertices[vertIndex] = points[i] + left * (Width * .5f);
        vertices[vertIndex + 1] = points[i] - left * (Width * .5f);

        var completionPercent = i / (float)(points.Length - 1);
        var v = 1 - Mathf.Abs(2 * completionPercent - 1);
        UVs[vertIndex] = new Vector2(0, v);
        UVs[vertIndex + 1] = new Vector2(1, v);

        if (i < points.Length - 1 || path.IsClosed)
        {
          triangles[triIndex] = vertIndex;
          triangles[triIndex + 1] = (vertIndex + 2) % vertices.Length;
          triangles[triIndex + 2] = vertIndex + 1;

          triangles[triIndex + 3] = vertIndex + 1;
          triangles[triIndex + 4] = (vertIndex + 2) % vertices.Length;
          triangles[triIndex + 5] = (vertIndex + 3) % vertices.Length;
        }

        vertIndex += 2;
        triIndex += 6;
      }
      
      var mesh = new Mesh
      {
        vertices = vertices,
        triangles = triangles,
        uv = UVs
      };
      mesh.RecalculateNormals();
      mesh.RecalculateTangents();
      mesh.Optimize();
      return mesh;
    }
    
  }
}