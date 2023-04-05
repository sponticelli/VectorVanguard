using UnityEngine;

namespace VectorVanguard.Pools
{
  public interface IPool
  {
    public void Initialization();
    public GameObject GetObject(Vector3 position, Quaternion rotation);
  }
}