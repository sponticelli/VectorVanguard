using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorGravityAbility : AActorAbility
  {
    [SerializeField] private float _gravityPower = 1;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;

    private AActorPhysics _physics;
    
    
    protected override void OnInitialization()
    {
      base.OnInitialization();
      _gravityDirection = _gravityDirection.normalized;
      _physics = _actor.Physics;
    }

    public override void Execute()
    {
      base.Execute();

      _physics.AddForce(_gravityDirection * _gravityPower);
    }
  }
}