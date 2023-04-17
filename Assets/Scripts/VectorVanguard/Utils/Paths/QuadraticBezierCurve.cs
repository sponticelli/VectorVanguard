using System;
using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  [Serializable]
  public class QuadraticBezierCurve : ACurve
  {
    [SerializeField] private Vector3 _controlPoint1;
    public Vector3 ControlPoint1
    {
      get => _controlPoint1;
      set
      {
        _controlPoint1 = value;
        _length = -1;
      }
    }

    public override Vector3 GetPoint(float t)
    {
      return (1 - t) * (1 - t) * _startPoint +
             2 * (1 - t) * t * _controlPoint1 +
             t * t * _endPoint;
    }
    
  }
}