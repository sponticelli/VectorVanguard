using System;
using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  [Serializable]
  public class CubicBezierCurve : ICurve
  {
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _controlPoint1;
    [SerializeField] private Vector3 _controlPoint2;
    [SerializeField] private Vector3 _endPoint;

    public Vector3 StartPoint
    {
      get => _startPoint;
      set => _startPoint = value;
    }

    public Vector3 ControlPoint1
    {
      get => _controlPoint1;
      set => _controlPoint1 = value;
    }

    public Vector3 ControlPoint2
    {
      get => _controlPoint2;
      set => _controlPoint2 = value;
    }

    public Vector3 EndPoint
    {
      get => _endPoint;
      set => _endPoint = value;
    }

    public CubicBezierCurve(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint)
    {
      _startPoint = startPoint;
      _controlPoint1 = controlPoint1;
      _controlPoint2 = controlPoint2;
      _endPoint = endPoint;
    }

    public Vector3 GetPoint(float t)
    {
      return (1 - t) * (1 - t) * (1 - t) * _startPoint +
             3 * (1 - t) * (1 - t) * t * _controlPoint1 +
             3 * (1 - t) * t * t * _controlPoint2 + 
             t * t * t * _endPoint;
    }

    public float Length(int samples = 100)
    {
      //Calculate the length of the curve by sampling it at a given number of points
      var length = 0f;
      var lastPoint = GetPoint(0);
      for (var i = 1; i <= samples; i++)
      {
        var t = i / (float) samples;
        var point = GetPoint(t);
        length += Vector3.Distance(point, lastPoint);
        lastPoint = point;
      }
      return length;
    }
  }
}