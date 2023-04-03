using UnityEngine;

namespace VectorVanguard.Utils
{
  [RequireComponent(typeof(Renderer))]
  public class RenderLayerSetter : MonoBehaviour
  {
        
    [SerializeField] private int _sortingOrder;
    [SerializeField] private int _sortingLayerID;
    
    
    private Renderer _renderer;

    private void Awake()
    {
      _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
      _renderer.sortingLayerID = _sortingLayerID;
      _renderer.sortingOrder = _sortingOrder;
    }
  }
}