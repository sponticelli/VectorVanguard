using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public class EarClippingTriangulator
  {
    private List<Vector2> _points;

    public EarClippingTriangulator(List<Vector2> points)
    {
      _points = points;
    }

    public int[] Triangulate()
    {
      List<int> indices = new List<int>();

      int pointCount = _points.Count;
      if (pointCount < 3)
      {
        return indices.ToArray();
      }

      int[] vertexIndices = new int[pointCount];
      for (int i = 0; i < pointCount; i++)
      {
        vertexIndices[i] = i;
      }

      int currentPoint = 0;
      int passCount = 0;
      while (pointCount > 2 && passCount < pointCount * 2)
      {
        int prev = vertexIndices[(currentPoint + pointCount - 1) % pointCount];
        int curr = vertexIndices[currentPoint % pointCount];
        int next = vertexIndices[(currentPoint + 1) % pointCount];

        if (IsEar(prev, curr, next))
        {
          indices.Add(prev);
          indices.Add(curr);
          indices.Add(next);

          for (int i = currentPoint; i < pointCount - 1; i++)
          {
            vertexIndices[i] = vertexIndices[i + 1];
          }

          pointCount--;
          passCount = 0;
        }
        else
        {
          passCount++;
        }

        currentPoint = (currentPoint + 1) % pointCount;
      }

      return indices.ToArray();
    }

    private bool IsEar(int a, int b, int c)
    {
      Vector2 A = _points[a];
      Vector2 B = _points[b];
      Vector2 C = _points[c];

      if (Area(A, B, C) >= 0)
      {
        return false;
      }

      for (int i = 0; i < _points.Count; i++)
      {
        if (i == a || i == b || i == c)
        {
          continue;
        }

        if (PointInTriangle(_points[i], A, B, C))
        {
          return false;
        }
      }

      return true;
    }

    private float Area(Vector2 A, Vector2 B, Vector2 C)
    {
      return (A.x - C.x) * (B.y - A.y) - (A.x - B.x) * (C.y - A.y);
    }

    private bool PointInTriangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C)
    {
      float a = Area(A, B, P);
      float b = Area(B, C, P);
      float c = Area(C, A, P);

      return a >= 0 && b >= 0 && c >= 0;
    }
  }
}