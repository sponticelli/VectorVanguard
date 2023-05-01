using UnityEngine;

namespace LiteNinja.PathMaster
{
  public static class BezierHelper
  {
    /// <summary>
    /// Quadratic Bezier curve
    /// </summary>
    /// <param name="startPoint">start point</param>
    /// <param name="controlPoint">control point</param>
    /// <param name="endPoint">end point</param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 EvaluateQuadratic(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint, float t)
    {
      var p0 = Vector2.Lerp(startPoint, controlPoint, t);
      var p1 = Vector2.Lerp(controlPoint, endPoint, t);
      return Vector2.Lerp(p0, p1, t);
    }

    /// <summary>
    /// Cubic Bezier curve
    /// </summary>
    /// <param name="startPoint">start point</param>
    /// <param name="controlPoint1">first control point</param>
    /// <param name="controlPoint2">second control point</param>
    /// <param name="endPoint">end point</param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 EvaluateCubic(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint, float t)
    {
      var p0 = EvaluateQuadratic(startPoint, controlPoint1, controlPoint2, t);
      var p1 = EvaluateQuadratic(controlPoint1, controlPoint2, endPoint, t);
      return Vector2.Lerp(p0, p1, t);
    }

    /// <summary>
    /// Calculate the length of a Cubic Bezier curve
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="controlPoint1"></param>
    /// <param name="controlPoint2"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static float CubicLength(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint)
    {
      var length = 0f;
      var previousPoint = startPoint;
      for (var i = 1; i <= 10; i++)
      {
        var t = i / 10f;
        var point = EvaluateCubic(startPoint, controlPoint1, controlPoint2, endPoint, t);
        length += Vector2.Distance(previousPoint, point);
        previousPoint = point;
      }

      return length;
    }
    
    /// <summary>
    /// Calculate the length of a quadratic Bezier curve
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="controlPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static float QuadraticLength(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint)
    {
      var length = 0f;
      var previousPoint = startPoint;
      for (var i = 1; i <= 10; i++)
      {
        var t = i / 10f;
        var point = EvaluateQuadratic(startPoint, controlPoint, endPoint, t);
        length += Vector2.Distance(previousPoint, point);
        previousPoint = point;
      }

      return length;
    }
        
  }
}