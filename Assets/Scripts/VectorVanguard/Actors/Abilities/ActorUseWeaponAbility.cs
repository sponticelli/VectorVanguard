using UnityEngine;
using VectorVanguard.Actors.Weapons;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorUseWeaponAbility : AActorAbility
  {
    [SerializeField] private string _actionID = "FIRE";
    [SerializeField] private AWeapon _weapon;
    [SerializeField] private float _actorSpeedMultiplier = 0.5f;
    
    private AActorPhysics _physics;
    private bool _isFiring;
    
    protected override void OnInitialization()
    {
      base.OnInitialization();
      _physics = _actor.Physics;
    }
    
    public override void EarlyExecute()
    {
      base.EarlyExecute();
      _isFiring = _actor.Input.IsPressed(_actionID) || _actor.Input.IsDown(_actionID);
    }
    
    public override void Execute()
    {
      base.Execute();
      if (_isFiring)
      {
        _weapon.Fire(_physics.GetSpeed() * _actorSpeedMultiplier);
      }
    }
  }
}