using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  public abstract class ACurve : ICurve
  {
    [SerializeField] protected Vector3 _startPoint;
    [SerializeField] protected Vector3 _endPoint;
    public Vector3 StartPoint
    {
      get => _startPoint;
      set
      {
        _startPoint = value;
        _length = -1;
      }
    }

    public Vector3 EndPoint
    {
      get => _endPoint;
      set
      {
        _endPoint = value;
        _length = -1;
      }
    }

    public abstract Vector3 GetPoint(float t);

    
    protected float _length = -1;
    protected int _samples = 0;
    
    public float Length(int samples = 100)
    {
      if (_length > 0 && _samples == samples)
      {
        return _length;
      }
      _samples  = samples;
      
      //Calculate the length of the curve by sampling it at a given number of points
      _length = 0f;
      var lastPoint = GetPoint(0);
      for (var i = 1; i <= samples; i++)
      {
        var t = i / (float) samples;
        var point = GetPoint(t);
        _length += Vector3.Distance(point, lastPoint);
        lastPoint = point;
      }
      return _length;
    }
  }
}