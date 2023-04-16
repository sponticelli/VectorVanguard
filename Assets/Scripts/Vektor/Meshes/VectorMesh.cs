using System;
using System.Collections.Generic;
using System.Linq;
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
    protected JoinType _joinType = JoinType.None;

    public enum JoinType
    {
      None,
      Fill,
      Weld
    }

    public JoinType Join
    {
      get => _joinType;
      set => _joinType = value;
    }

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

      RemoveDuplicateSegments();

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

      var pairedSegments = FindPairedSegments();

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
        p0.z = start.z;
        var p1 = start + right + up;
        p1.z = start.z; 
        var p2 = end + right - up;
        p2.z = end.z;
        var p3 = end - right - up;
        p3.z = end.z;
        

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

      // Join the paired segments
      switch (_joinType)
      {
        case JoinType.None:
          break;
        case JoinType.Fill:
          FillJoin(pairedSegments, triangles);
          break;
        case JoinType.Weld:
          WeldJoin(pairedSegments, triangles, vertices);
          break;
      }

      _mesh = new Mesh
      {
        vertices = vertices.ToArray(),
        triangles = triangles.ToArray()
      };

      return _mesh;
    }

    private void FillJoin(List<Tuple<Segment, Segment>> pairedSegments, List<int> triangles)
    {
      foreach (var (segment1, segment2) in pairedSegments)
      {
        var segment1Index = Array.IndexOf(_segments, segment1) * 4;
        var segment2Index = Array.IndexOf(_segments, segment2) * 4;
        var isSegment1Start = (segment1.Start == segment2.Start || segment1.Start == segment2.End);
        var isSegment2Start = (segment2.Start == segment1.Start || segment2.Start == segment1.End);

        segment1Index += isSegment1Start ? 0 : 2;
        segment2Index += isSegment2Start ? 0 : 2;

        var p0Index = segment1Index;
        var p1Index = segment1Index + 1;
        var p2Index = segment2Index + 1;
        var p3Index = segment2Index;

        //add a triangle p0, p1, p2
        triangles.Add(p0Index);
        triangles.Add(p1Index);
        triangles.Add(p2Index);

        //add a triangle p0, p1, p3
        triangles.Add(p0Index);
        triangles.Add(p1Index);
        triangles.Add(p3Index);
      }
    }

    private void WeldJoin(List<Tuple<Segment, Segment>> pairedSegments, List<int> triangles, List<Vector3> vertices)
    {
      foreach (var (segment1, segment2) in pairedSegments)
      {
        // Determine which endpoints of the segments should be connected
        var isSegment1Start = (segment1.Start == segment2.Start || segment1.Start == segment2.End);
        var isSegment2Start = (segment2.Start == segment1.Start || segment2.Start == segment1.End);
        
        var commonPoint = isSegment1Start ? segment1.Start : segment1.End;

        // Get the indices of the vertices at the endpoints
        var segment1Index = Array.IndexOf(_segments, segment1) * 4 + (isSegment1Start ? 0 : 2);
        var segment2Index = Array.IndexOf(_segments, segment2) * 4 + (isSegment2Start ? 0 : 2);
        
        var p0Index = segment1Index;
        var p1Index = segment1Index + 1;
        var p2Index = segment2Index + 1;
        var p3Index = segment2Index;
        
        //determine the direction of the segments
        var segment1Direction = isSegment1Start ? -segment1.Direction : segment1.Direction;
        var segment2Direction = isSegment2Start ? -segment2.Direction : segment2.Direction;

        AddIntersection(triangles, vertices, p0Index, segment1Direction, p2Index, segment2Direction, commonPoint);
        AddIntersection(triangles, vertices, p1Index, segment1Direction, p3Index, segment2Direction, commonPoint);
        
      }

    }

    private static void AddIntersection(List<int> triangles, List<Vector3> vertices, int p0Index, Vector3 segment1Direction, int p2Index,
      Vector3 segment2Direction, Vector3 commonPoint)
    {
      if (!LineLineIntersection(vertices[p0Index], segment1Direction, vertices[p2Index], segment2Direction,
            out var intersectionPoint1)) return;
      var intersectionIndex = vertices.IndexOf(intersectionPoint1);

      if (intersectionIndex == -1)
      {
        intersectionIndex = vertices.Count;
        vertices.Add(intersectionPoint1);
      }
      
      var commonPointIndex = vertices.IndexOf(commonPoint);
      if (commonPointIndex == -1)
      {
        commonPointIndex = vertices.Count;
        vertices.Add(commonPoint);
      }

      //add a triangle p0, p2, intersection
      triangles.Add(p0Index);
      triangles.Add(p2Index);
      triangles.Add(intersectionIndex);
      
      //add a triangle p0, p2, commonPoint
      triangles.Add(p0Index);
      triangles.Add(p2Index);
      triangles.Add(commonPointIndex);
    }

    public static bool LineLineIntersection(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, out Vector3 intersection)
    {
      intersection = Vector3.zero;

      var crossVec = Vector3.Cross(lineVec1, lineVec2);
      var pointVec = linePoint2 - linePoint1;

      var planarFactor = Vector3.Dot(pointVec, crossVec);

      if (!(Mathf.Abs(planarFactor) < 0.0001f) || !(crossVec.sqrMagnitude > 0.0001f)) return false;
      
      var crossPointVec = Vector3.Cross(pointVec, lineVec2);

      var s = Vector3.Dot(crossPointVec, crossVec) / crossVec.sqrMagnitude;

      intersection = linePoint1 + (lineVec1 * s);
      return true;

    }

    private void RemoveDuplicateSegments()
    {
      // Remove duplicate segments by removing the segment that has the same start and end points,or the end point is the same as the start point and the start point is the same as the end point
      var segments = new List<Segment>();
      foreach (var segment in _segments)
      {
        if (segments.Any(s => s.Start == segment.Start && s.End == segment.End) ||
            segments.Any(s => s.Start == segment.End && s.End == segment.Start))
        {
          continue;
        }

        segments.Add(segment);
      }

      _segments = segments.ToArray();
    }

    private List<Tuple<Segment, Segment>> FindPairedSegments()
    {
      var pairedSegments = new List<Tuple<Segment, Segment>>();
      for (var i = 0; i < _segments.Length; i++)
      {
        for (var j = i + 1; j < _segments.Length; j++)
        {
          var a = _segments[i];
          var b = _segments[j];


          if (a.Start == b.Start || a.Start == b.End ||
              a.End == b.Start || a.End == b.End)
          {
            pairedSegments.Add(new Tuple<Segment, Segment>(a, b));
          }
        }
      }

      return pairedSegments;
    }

  }
}