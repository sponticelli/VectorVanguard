using System;
using UnityEngine;

namespace VectorVanguard.Utils
{
  [Serializable]
  public class ImpactInfo
  {
    public Vector3 Position;
    public Vector3 Direction;
    public float Force;
    
    public ImpactInfo(Vector3 position, Vector3 direction, float force)
    {
      Position = position;
      Direction = direction;
      Force = force;
    }
    
    public ImpactInfo() : this(Vector3.zero, Vector3.zero, 0f)
    {
    }


  }
}