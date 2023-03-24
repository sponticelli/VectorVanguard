using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorGravityAbility : AActorAbility
  {
    [SerializeField] private float _gravityPower = 1;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;

    private ActorPhysics _physics;
    
    
    protected override void OnInitialization()
    {
      base.OnInitialization();
      _gravityDirection = _gravityDirection.normalized;
      _physics = _actor.GetComponent<ActorPhysics>();
    }

    public override void Execute()
    {
      base.Execute();

      _physics.AddForce(_gravityDirection * _gravityPower);
    }
  }
}