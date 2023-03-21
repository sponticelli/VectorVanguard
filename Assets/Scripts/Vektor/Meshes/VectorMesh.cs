using System;
using System.Collections.Generic;
using UnityEngine;


namespace Vektor.Meshes
{
  /// <summary>
  /// Generates a mesh from a list of segments
  /// </summary>
  public class VectorMesh
  {
    protected Segment[] _segments;
    protected float _lineWidth = 0.1f;
    protected Mesh _mesh;

    public Segment[] segments
    {
      get => _segments;
      set => _segments = value;
    }

    public float lineWidth
    {
      get => _lineWidth;
      set => _lineWidth = value;
    }

    public Mesh mesh => _mesh;

    public VectorMesh()
    {
      _segments = Array.Empty<Segment>();
    }

    public VectorMesh(Segment[] segments, float lineWidth)
    {
      _segments = segments;
      _lineWidth = lineWidth;
    }

    public Mesh Generate()
    {
      return CreateMesh();
    }

    public virtual Mesh CreateMesh()
    {
      if (_segments == null || _segments.Length == 0)
      {
        _mesh = null;
        return null;
      }

      var vertices = new List<Vector3>();
      var triangles = new List<int>();
      var uvs = new List<Vector2>();
      var normals = new List<Vector3>();

      var segmentCount = _segments.Length;
      var vertexCount = segmentCount * 4;
      var triangleCount = segmentCount * 6;

      vertices.Capacity = vertexCount;
      triangles.Capacity = triangleCount;
      uvs.Capacity = vertexCount;
      normals.Capacity = vertexCount;

      var index = 0;
      var uvStep = 1f / segmentCount;
      var uv = 0f;
      var normal = Vector3.forward;

      foreach (var segment in _segments)
      {
        var start = segment.Start;
        var end = segment.End;

        var direction = end - start;
        var length = direction.magnitude;
        var halfWidth = _lineWidth * 0.5f;

        var right = Vector3.Cross(direction, normal).normalized * halfWidth;
        var up = Vector3.Cross(right, direction).normalized * halfWidth;

        var p0 = start - right + up;
        var p1 = start + right + up;
        var p2 = end + right - up;
        var p3 = end - right - up;

        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        uvs.Add(new Vector2(uv, 0));
        uvs.Add(new Vector2(uv, 1));
        uvs.Add(new Vector2(uv + uvStep, 1));
        uvs.Add(new Vector2(uv + uvStep, 0));

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        index += 4;
        uv += uvStep;
      }

      _mesh = new Mesh
      {
        vertices = vertices.ToArray(),
        triangles = triangles.ToArray()
      };

      return _mesh;
    }
  }
}