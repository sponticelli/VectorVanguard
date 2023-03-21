using System;
using UnityEngine;

namespace Vektor
{
  [Serializable]
  public class Segment
  {
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    
    public Vector3 Start
    {
      get => _start;
      set => _start = value;
    }
    
    public Vector3 End
    {
      get => _end;
      set => _end = value;
    }

    public Vector3 Direction=> (_end - _start).normalized;
    

    public Segment()
    {
      _start = Vector3.zero;
      _end = Vector3.zero;
    }
    public Segment(Vector2 start, Vector2 end)
    {
      _start = start;
      _end = end;
    }
    
    public Segment(Segment segment)
    {
      _start = segment.Start;
      _end = segment.End;
    }
    
    public Segment Clone()
    {
      return new Segment(this);
    }
    
    
  }
}