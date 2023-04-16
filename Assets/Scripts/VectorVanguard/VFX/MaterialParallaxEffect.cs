using UnityEngine;

namespace VectorVanguard.VFX
{
  [RequireComponent(typeof(MeshRenderer))]
  public class MaterialParallaxEffect : MonoBehaviour
  {
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _speed = 0.1f;
    
    private Vector2 _offset;

    private void Awake()
    {
      if (_meshRenderer == null)
      {
        _meshRenderer = GetComponent<MeshRenderer>();
      }
    }

    private void Update()
    {
      Execute();
    }
    
    private void Execute()
    {
      _offset = transform.position * _speed;
      _meshRenderer.material.SetFloat("_OffsetX", _offset.x);
      _meshRenderer.material.SetFloat("_OffsetY", _offset.y);
    }
    
  }
}