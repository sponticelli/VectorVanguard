using UnityEngine;

namespace VectorVanguard.Pools
{
  public class MultiPool : AExpandablePool
  {
    [SerializeField] private GameObject[] _prefabs;


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

    protected override void AddObject(int i)
    {
      var prefab = _prefabs[i % _prefabs.Length];
      var obj = Instantiate(prefab, transform);
      obj.SetActive(false);
      obj.name = $"{_poolTag} {i}";
      _pool.Add(obj);
    }
  }
}