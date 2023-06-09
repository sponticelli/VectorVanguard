using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorRotateAbility : AActorAbility
  {
    [SerializeField]
    private float _rotationPower = 200;
    [SerializeField]
    private float _angularDecayRate = 0.7f;
    
    [SerializeField]
    private bool _invertDirection = true;
    
    private float _direction;
    private float _angularVelocity;
    private AActorPhysics _physics;
    private float _directionMultiplier = 1;

    protected override void OnInitialization()
    {
      base.OnInitialization();
      _physics = _actor.Physics;
      _directionMultiplier = _invertDirection ? -1 : 1;
    }
    
    public override void EarlyExecute()
    {
      base.EarlyExecute();
      _direction = _directionMultiplier * _actor.Input.GetMovementDirection().x;
    }

    public override void Execute()
    {
      base.Execute();
      _angularVelocity = _direction * _rotationPower * Time.deltaTime;
      
      if (_direction == 0)
      {
        _angularVelocity *= _angularDecayRate;
      }
      
      _physics.AddRotationForce(_angularVelocity);
    }
  }
}