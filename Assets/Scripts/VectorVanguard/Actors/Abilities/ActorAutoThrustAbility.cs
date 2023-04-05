using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorAutoThrustAbility : AActorAbility
  {
    [SerializeField] private float _thrustForce;
    [SerializeField] private Vector3 _thrustDirection;

    public float ThrustForce
    {
      get => _thrustForce;
      set => _thrustForce = value;
    }
    
    public Vector3 ThrustDirection
    {
      get => _thrustDirection;
      set => _thrustDirection = value;
    }
    
    private AActorPhysics _physics;
    public override void Initialization(Actor actor)
    {
      base.Initialization(actor);
      _physics = _actor.Physics;
    }
    
    public override void Execute()
    {
      base.Execute();
      _physics.AddForce(_thrustDirection * _thrustForce);
    }
  }
}