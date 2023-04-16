using System.Collections.Generic;
using UnityEngine;

namespace VectorVanguard.Pools
{
  public class PoolManager : MonoBehaviour
  {
    private static PoolManager _instance;
    public static PoolManager Instance => _instance;
    
    private readonly List<APool> _pools = new();
    private Dictionary<PoolTag, APool> _poolDictionary = new();

    private void Awake()
    {
      if (_instance != null && _instance != this)
      {
        Destroy(gameObject);
        return;
      }
      _instance = this;
      
      //Get all pools in children
      _pools.AddRange(GetComponentsInChildren<APool>());
      foreach (var pool in _pools)
      {
        pool.Initialization();
        _poolDictionary.Add(pool.PoolTag, pool);
      }
    }
    
    public GameObject GetObject(PoolTag tag, Vector3 position, Quaternion rotation)
    {
      if (_poolDictionary.TryGetValue(tag, out var pool))
      {
        return pool.GetObject(position, rotation);
      }
      Debug.LogError($"Pool with tag {tag} does not exist");
      return null;
    }
    
    
    
    public GameObject GetObject(PoolTag tag, Vector3 position)
    {
      return GetObject(tag, position, Quaternion.identity);
    }
    
    public GameObject GetObject(PoolTag tag)
    {
      return GetObject(tag, Vector3.zero, Quaternion.identity);
    }
  }
}