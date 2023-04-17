using UnityEngine;

namespace VectorVanguard.Utils.Paths
{
  public interface ICurve
  {
    Vector3 StartPoint { get; set; }
    Vector3 EndPoint { get; set; }
    Vector3 GetPoint(float t);

    float Length(int samples = 100);

  }
}