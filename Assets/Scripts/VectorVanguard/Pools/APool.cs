using UnityEngine;

namespace VectorVanguard.Pools
{
  public abstract class APool : MonoBehaviour, IPool
  {
    [SerializeField] protected string _tag;
    public string Tag => _tag;
    
    public abstract void Initialization();
    public abstract GameObject GetObject(Vector3 position, Quaternion rotation);
  }
}