using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  /// <summary>
  /// A simple component that makes an object follow a path.
  /// </summary>
  public class FollowPath : MonoBehaviour
  {
    [SerializeField] private MonoPath2D _path;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _align = false;
    
    private float _distance = 0f;
    
    public MonoPath2D Path
    {
      get => _path;
      set => _path = value;
    }
    
    public float Speed
    {
      get => _speed;
      set => _speed = value;
    }
    
    public bool Align
    {
      get => _align;
      set => _align = value;
    }
    
    private void Start()
    {
      _distance = 0f;
    }

    private void Update()
    {
      Move();
      Alignment();
    }
    
    private void Move()
    {
      transform.position = _path.GetPositionByDistance(_distance, true);
      _distance += _speed * Time.deltaTime;
    }
    
    private void Alignment()
    {
      if (!_align) return;
      
      var direction = (_path.GetPositionByDistance(_distance, true) - (Vector2) transform.position).normalized;
      var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
  }
}