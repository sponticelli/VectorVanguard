namespace VectorVanguard.Actors.Abilities
{
  public class ActorDeathAbility : AActorAbility
  {
    
    protected ActorHealth _actorHealth;
    public override void Initialization(Actor actor)
    {
      base.Initialization(actor);
      _actorHealth = _actor.ActorAbilities.GetAbility<ActorHealth>();
        
      if (_actorHealth) _actorHealth.OnDeath.AddListener(OnDeath);
 
    }

    protected virtual void OnDeath()
    {
      _actor.gameObject.SetActive(false);
    }
  }
}