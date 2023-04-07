using System.Collections;
using LiteNinja.SOA.Events;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorDeathAbility : AActorAbility
  {
    [SerializeField] private Vector3Event _onDeathPosition;
    protected ActorHealth _actorHealth;
    
    public UnityEvent OnDeath;
    
    protected bool _isDead;
    
    public override void Initialization(Actor actor)
    {
      base.Initialization(actor);
      _isDead = false;
      _actorHealth = _actor.ActorAbilities.GetAbility<ActorHealth>();
        
      if (_actorHealth) _actorHealth.OnHealthChanged.AddListener(OnDamage);
 
    }

    private void OnDamage(float health)
    {
      if (health > 0 || _isDead) return;
      _isDead = true;
      OnDeath.Invoke();
      if (_actorHealth) _actorHealth.OnHealthChanged.RemoveListener(OnDamage);
      _onDeathPosition?.Raise(_actor.transform.position);
      _actor.gameObject.SetActive(false);
    }
  }
}