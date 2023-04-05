using VectorVanguard.Actors.Abilities;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors
{
  public class DamageableActor : Actor, IDamageable
  {

    private IDamageable _actorHealth;

    public void TakeDamage(float damage)
    {
      _actorHealth?.TakeDamage(damage);
    }

    private void Awake()
    {
      Initialization();
    }
    
    protected override void Initialization()
    {
      base.Initialization();
      _actorHealth = ActorAbilities.GetAbility<ActorHealth>();
    }
  }
}