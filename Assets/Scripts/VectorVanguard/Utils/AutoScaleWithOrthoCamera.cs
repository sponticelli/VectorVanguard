using UnityEngine;

namespace VectorVanguard.Utils
{
  public class AutoScaleWithOrthoCamera : MonoBehaviour
  {
    [field: SerializeField] private Camera Camera { get; set; }
    [SerializeField] private float _scale = 0.2f;

    private void Start()
    {
      SetScale();
    }

    public void SetScale()
    {
      transform.localScale = Vector3.one * Camera.orthographicSize * _scale; 
    }
  }
}