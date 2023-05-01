using UnityEngine;

namespace VectorVanguard.Utils
{
  public static class GeometryHelper
  {
    /// <summary>
    /// Calc the intersection point of two lines. The lines are defined by two nodes each.
    /// </summary>
    /// <param name="A1">first point of line A</param>
    /// <param name="A2">second point of line A</param>
    /// <param name="B1">first point of line B</param>
    /// <param name="B2">second point of line B</param>
    /// <param name="found">true if line A intersects line B</param>
    /// <returns>the intersection point</returns>
    public static Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
      var tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);
 
      if (tmp == 0)
      {
        // No solution!
        found = false;
        return Vector2.zero;
      }
 
      var mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
 
      found = true;
 
      return new Vector2(
        B1.x + (B2.x - B1.x) * mu,
        B1.y + (B2.y - B1.y) * mu
      );
    }
  }
}