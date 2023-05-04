using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public static class ConvexHull
  {
    public static List<Vector2> CalculateConvexHull(List<Vector2> points)
    {
      if (points.Count < 3)
      {
        return points;
      }

      var leftMostIndex = 0;
      for (var i = 1; i < points.Count; i++)
      {
        if (points[i].x < points[leftMostIndex].x)
        {
          leftMostIndex = i;
        }
      }

      var hullPoints = new List<Vector2>();
      var currentPointIndex = leftMostIndex;
      int nextPointIndex;

      do
      {
        hullPoints.Add(points[currentPointIndex]);
        nextPointIndex = (currentPointIndex + 1) % points.Count;

        for (var i = 0; i < points.Count; i++)
        {
          if (IsLeftOfLine(points[currentPointIndex], points[i], points[nextPointIndex]) > 0)
          {
            nextPointIndex = i;
          }
        }

        currentPointIndex = nextPointIndex;
      } while (currentPointIndex != leftMostIndex);

      return hullPoints;
    }

    private static float IsLeftOfLine(Vector2 a, Vector2 b, Vector2 c)
    {
      return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
    }
  }
}