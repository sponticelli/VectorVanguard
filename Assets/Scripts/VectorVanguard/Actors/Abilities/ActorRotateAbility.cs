using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorRotateAbility : AActorAbility
  {
    [SerializeField]
    private float _maxSpeed = 5f;
    [SerializeField]
    private float _acceleration = 1f;
    [SerializeField]
    private float _deceleration = 1f;
    
    
    private float _direction;
    private float _currentSpeed;
    private ActorPhysics _physics;

    protected override void OnInitialization()
    {
      base.OnInitialization();
      _physics = _actor.GetComponent<ActorPhysics>();
    }
    
    public override void EarlyExecute()
    {
      base.EarlyExecute();
      _direction = _actor.Input.GetMovementDirection().x;
    }

    public override void Execute()
    {
      base.Execute();
      if (_direction == 0)
      {
        _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _deceleration * Time.deltaTime);
        if (_currentSpeed < 0.01f) _currentSpeed = 0;
      }
      else
      {
        _currentSpeed += _direction * _acceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, -_maxSpeed, _maxSpeed);
      }
      
      _physics.AddRotation(_currentSpeed);
    }
  }
}