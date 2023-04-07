using UnityEngine;
using VectorVanguard.Actors.Abilities;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors
{
  public class DamageableActor : Actor, IDamageable, IImpactable
  {

    private IDamageable _actorDamageable;
    private IImpactable _actorImpactable;

    public void TakeDamage(float damage)
    {
      _actorDamageable?.TakeDamage(damage);
    }
    
    public void Impact(ImpactInfo impactInfo)
    {
      Physics.AddExternalForce(impactInfo.Direction * impactInfo.Force);
      _actorImpactable?.Impact(impactInfo);
    }

    private void Awake()
    {
      Initialization();
    }
    
    protected override void Initialization()
    {
      base.Initialization();
      _actorDamageable = ActorAbilities.GetAbility<ActorHealth>();
      _actorImpactable = ActorAbilities.GetAbility<ActorHealth>();
    }

    
  }
}