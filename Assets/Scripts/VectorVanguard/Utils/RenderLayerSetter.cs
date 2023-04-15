using System.Linq;
using UnityEngine;

namespace VectorVanguard.Utils
{
  [ExecuteInEditMode]
  [RequireComponent(typeof(Renderer))]
  public class RenderLayerSetter : MonoBehaviour
  {
        
    [SerializeField] private int _sortingOrder;
    [SerializeField] private int _sortingLayerID;
    
    
    private Renderer _renderer;
    
    public SortingLayer SortingLayer
    {
      get => SortingLayer.layers.FirstOrDefault(layer => layer.id == _sortingLayerID);
      set
      {
        _sortingLayerID = value.id;
        SetSortingInfo();
      }
    }

    public int SortingOrder
    {
      get => _sortingOrder;
      set
      {
        _sortingOrder = value;
        SetSortingInfo();
      }
    }

    private void Awake()
    {
      _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
      SetSortingInfo();
    }

    private void SetSortingInfo()
    {
      if (_renderer == null)
      {
        _renderer = GetComponent<Renderer>();
      };
      _renderer.sortingLayerID = _sortingLayerID;
      _renderer.sortingOrder = _sortingOrder;
    }
    
  }
}