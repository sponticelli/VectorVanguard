using System.Collections.Generic;
using UnityEngine;

namespace VectorVanguard.Pools
{
  public abstract class AExpandablePool : APool
  {
    [SerializeField] protected int _size = 10;
    [SerializeField] protected bool _expandable = true;
    [SerializeField] protected int _maxExpandSize = 100;
    [SerializeField] protected int _expandSize = 10;
    
    protected readonly List<GameObject> _pool = new();
    protected readonly  List<GameObject> _activeObjects = new();
    
    public override void Initialization()
    {
      for (var i = 0; i < _size; i++)
      {
        AddObject(i);
      }
    }

    public override GameObject GetObject(Vector3 position, Quaternion rotation)
    {
      Enqueue();
      if (_pool.Count == 0) return null;
      var lastIndex = _pool.Count - 1;
      var obj = _pool[lastIndex];
      _pool.RemoveAt(lastIndex);
      obj.transform.position = position;
      obj.transform.rotation = rotation;
      obj.SetActive(true);
      _activeObjects.Add(obj);
      return obj;
    }

    private void Enqueue()
    {
      // if _pool is empty, move all inactive _activeObjects to _pool
      if (_pool.Count != 0) return;
      for (var i = 0; i < _activeObjects.Count; i++)
      {
        if (_activeObjects[i].activeSelf) continue;
        _pool.Add(_activeObjects[i]);
        _activeObjects.RemoveAt(i);
        i--;
      }

      if (_pool.Count == 0)
      {
        Expand();
      }
    }

    protected abstract void Expand();
    protected abstract void AddObject(int i);
  }
}