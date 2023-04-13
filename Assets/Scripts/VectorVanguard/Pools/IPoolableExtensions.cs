using UnityEngine;

namespace VectorVanguard.Pools
{
  public static class IPoolableExtensions
  {
    public static void Despawn(this IPoolable poolable)
    {
      poolable.Despawn();
    }
    
    public static void Spawn(this IPoolable poolable)
    {
      poolable.Spawn();
    }

    public static void Despawn(this GameObject gameObject)
    {
      //Find all the components that implement IPoolable
      var poolables = gameObject.GetComponents<IPoolable>();
      //If there are any, call Despawn on each of them
      foreach (var poolable in poolables)
      {
        poolable.Despawn();
      }
    }
    
    public static void Spawn(this GameObject gameObject)
    {
      //Find all the components that implement IPoolable
      var poolables = gameObject.GetComponents<IPoolable>();
      //If there are any, call Spawn on each of them
      foreach (var poolable in poolables)
      {
        poolable.Spawn();
      }
    }
  }
}