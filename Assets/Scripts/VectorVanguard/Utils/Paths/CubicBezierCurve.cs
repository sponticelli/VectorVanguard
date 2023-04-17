using System;
using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  [Serializable]
  public class CubicBezierCurve : ACurve
  {
    [SerializeField] private Vector3 _controlPoint1;
    [SerializeField] private Vector3 _controlPoint2;

    public Vector3 ControlPoint1
    {
      get => _controlPoint1;
      set
      {
        _controlPoint1 = value;
        _length = -1;
      }
    }

    public Vector3 ControlPoint2
    {
      get => _controlPoint2;
      set
      {
        _controlPoint2 = value;
        _length = -1;
      }
    }

    public CubicBezierCurve(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint)
    {
      _startPoint = startPoint;
      _controlPoint1 = controlPoint1;
      _controlPoint2 = controlPoint2;
      _endPoint = endPoint;
    }

    public override Vector3 GetPoint(float t)
    {
      return (1 - t) * (1 - t) * (1 - t) * _startPoint +
             3 * (1 - t) * (1 - t) * t * _controlPoint1 +
             3 * (1 - t) * t * t * _controlPoint2 + 
             t * t * t * _endPoint;
    }
  }
}