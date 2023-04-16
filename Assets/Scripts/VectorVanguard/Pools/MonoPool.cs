using UnityEngine;

namespace VectorVanguard.Pools
{
  public class MonoPool : AExpandablePool
  {
    [SerializeField] private GameObject _prefab;

    protected override void AddObject(int i)
    {
      var obj = Instantiate(_prefab, transform);
      obj.SetActive(false);
      obj.name = $"{_poolTag} {i}";
      _pool.Add(obj);
    }
    
    protected override void Expand()
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
        AddObject(i+_size);
      }
      _size += expandSize;
    }
  }
}