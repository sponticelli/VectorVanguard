using UnityEngine;

namespace VectorVanguard.Pools
{
  public abstract class APool : MonoBehaviour, IPool
  {
    [SerializeField] protected PoolTag _poolTag;
    public PoolTag PoolTag => _poolTag;
    
    public abstract void Initialization();
    public abstract GameObject GetObject(Vector3 position, Quaternion rotation);
  }
}