using System.Collections.Generic;
using UnityEngine;

namespace VectorVanguard.Pools
{
  public class Pool : MonoBehaviour
  {
    [SerializeField] private string _tag;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _size = 10;
    [SerializeField] private bool _expandable = true;
    [SerializeField] private int _maxExpandSize = 100;
    [SerializeField] private int _expandSize = 10;
    
    public string Tag => _tag;
    
    private readonly List<GameObject> _pool = new();
    private readonly  List<GameObject> _activeObjects = new();

    public void Initialization()
    {
      for (var i = 0; i < _size; i++)
      {
        var obj = Instantiate(_prefab, transform);
        obj.SetActive(false);
        obj.name = $"{_tag} {i}";
        _pool.Add(obj);
      }
    }
    
    public GameObject GetObject(Vector3 position, Quaternion rotation)
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

    private void Expand()
    {
      if (!_expandable) return;
      if (_pool.Count >= _maxExpandSize) return;
      var expandSize = _expandSize;
      if (_pool.Count + expandSize > _maxExpandSize)
      {
        expandSize = _maxExpandSize - _pool.Count;
      }
      for (var i = 0; i < expandSize; i++)
      {
        var obj = Instantiate(_prefab, transform);
        obj.SetActive(false);
        obj.name = $"{_tag} {i+_size}";
        _pool.Add(obj);
      }
      _size += expandSize;
    }
  }
}