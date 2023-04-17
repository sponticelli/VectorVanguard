using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  [Serializable]
  public class Path<T> where T : ICurve
  {
    [SerializeField] private List<T> _curves;
    [SerializeField] private bool _isClosed;
    
    public List<T> Curves
    {
      get => _curves;
      set => _curves = value;
    }
    
    public bool IsClosed
    {
      get => _isClosed;
      set => _isClosed = value;
    }
    
    public Path(List<T> curves, bool isClosed)
    {
      _curves = curves;
      _isClosed = isClosed;
    }
    
    public Vector3 GetPoint(float t)
    {
      switch (_curves.Count)
      {
        case 0:
          return Vector3.zero;
        case 1:
          return _curves[0].GetPoint(t);
      }

      var curveCount = _curves.Count;
      var curveIndex = Mathf.FloorToInt(t * curveCount);
      curveIndex = Mathf.Clamp(curveIndex, 0, curveCount - 1);
      var curve = _curves[curveIndex];
      var curveT = t * curveCount - curveIndex;
      return curve.GetPoint(curveT);
    }
    
    //Get the point given a t (between 0 and 1) and keeping in mind the length of the curve
    public Vector3 GetPointByLength(float t)
    {
      var length = Length();
      var targetLength = t * length;
      var currentLength = 0f;
      var lastPoint = GetPoint(0);
      for (var i = 1; i <= 100; i++)
      {
        var point = GetPoint(i / 100f);
        currentLength += Vector3.Distance(point, lastPoint);
        if (currentLength >= targetLength)
        {
          return point;
        }
        lastPoint = point;
      }
      return GetPoint(1);
    }
    
    
    public float Length(int samples = 100)
    {
      //Calculate the length of the curve by sampling it at a given number of points
      return _curves.Sum(curve => curve.Length(samples));
    }
    
  }
}