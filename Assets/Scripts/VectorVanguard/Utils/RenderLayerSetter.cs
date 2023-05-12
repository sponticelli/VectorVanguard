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
    [SerializeField] private bool _useZOrder = true;
    [SerializeField] private float _zOrderFactor = 0.01f;
    
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
    
    
    public bool UseZOrder
    {
      get => _useZOrder;
      set
      {
        _useZOrder = value;
        SetSortingInfo();
      }
    }
    
    public float ZOrderFactor
    {
      get => _zOrderFactor;
      set
      {
        _zOrderFactor = value;
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

    public void SetSortingInfo()
    {
      if (_renderer == null)
      {
        _renderer = GetComponent<Renderer>();
      };
      _renderer.sortingLayerID = _sortingLayerID;
      _renderer.sortingOrder = _sortingOrder;

      if (_useZOrder)
      {
        // Calculate Z position based on sorting layer and sorting order within the layer
        var layerZ = SortingLayer.layers.Length - SortingLayer.layers.ToList().IndexOf(SortingLayer);
        var zPosition = layerZ + _sortingOrder * _zOrderFactor;
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
      }

      var sortingLayer = SortingLayer.layers.FirstOrDefault(layer => layer.id == _sortingLayerID);
      Debug.Log(
        $"{gameObject.name} Sorting layer set to {sortingLayer.name} and order to {_renderer.sortingOrder}.");
    }
    
    
  }
}