using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  /// <summary>
  /// Generate a mesh that is the border of a path, given a width
  /// </summary>
  public class BorderedPathMeshBuilder : IMeshBuilder
  {
    public float Width { get; set; } = 1f;
    public float Spacing { get; set; } = 0.01f;
    public float BorderWidth { get; set; } = 0.1f;

    public BorderedPathMeshBuilder(float spacing, float width, float borderWidth)
    {
      Width = width;
      Spacing = spacing;
      BorderWidth = borderWidth;
    }

    public Mesh BuildMesh(Path2D path)
  {
    var points = path.GetEquidistantNodes(Spacing);
    var vertices = new Vector3[points.Length * 4];
    var numTriangles = ((points.Length-1) * 2) + ((path.IsClosed) ? 2 : 0);
    var triangles = new int[numTriangles * 3 * 2];
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

      Vector3 innerLeft = points[i] + left * (Width * .5f);
      Vector3 outerLeft = points[i] + left * (Width * 0.5f + BorderWidth);
      Vector3 innerRight = points[i] - left * (Width * .5f);
      Vector3 outerRight = points[i] - left * (Width * 0.5f + BorderWidth);

      vertices[vertIndex] = innerLeft;
      vertices[vertIndex + 1] = outerLeft;
      vertices[vertIndex + 2] = innerRight;
      vertices[vertIndex + 3] = outerRight;

      var completionPercent = i / (float)(points.Length - 1);
      var v = 1 - Mathf.Abs(2 * completionPercent - 1);
      UVs[vertIndex] = new Vector2(0, v);
      UVs[vertIndex + 1] = new Vector2(1, v);
      UVs[vertIndex + 2] = new Vector2(0, v);
      UVs[vertIndex + 3] = new Vector2(1, v);

      if (i < points.Length - 1 || path.IsClosed)
      {
        // Left side triangles
        triangles[triIndex] = vertIndex;
        triangles[triIndex + 1] = (vertIndex + 4) % vertices.Length;
        triangles[triIndex + 2] = vertIndex + 1;

        triangles[triIndex + 3] = vertIndex + 1;
        triangles[triIndex + 4] = (vertIndex + 4) % vertices.Length;
        triangles[triIndex + 5] = (vertIndex + 5) % vertices.Length;

        // Right side triangles
        triangles[triIndex + 6] = vertIndex + 2;
        triangles[triIndex + 7] = (vertIndex + 6) % vertices.Length;
        triangles[triIndex + 8] = vertIndex + 3;

        triangles[triIndex + 9] = vertIndex + 3;
        triangles[triIndex + 10] = (vertIndex + 6) % vertices.Length;
        triangles[triIndex + 11] = (vertIndex + 7) % vertices.Length;
      }

      vertIndex += 4;
      triIndex += 12;
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