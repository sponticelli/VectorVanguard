using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorDeathAbility : AActorAbility
  {
    [SerializeField] private float _delayedDestroyTime = 0.1f;
    protected ActorHealth _actorHealth;
    
    public UnityEvent OnDeath;
    
    public override void Initialization(Actor actor)
    {
      base.Initialization(actor);
      _actorHealth = _actor.ActorAbilities.GetAbility<ActorHealth>();
        
      if (_actorHealth) _actorHealth.OnHealthChanged.AddListener(OnDamage);
 
    }

    private void OnDamage(float health)
    {
      if (!(health <= 0)) return;
      OnDeath.Invoke();
      if (_actorHealth) _actorHealth.OnHealthChanged.RemoveListener(OnDamage);
      
      //Disable the actor after 1 second
      StartCoroutine(DelayedDestroy(_delayedDestroyTime));
      
    }

    private IEnumerator DelayedDestroy(float f)
    {
      yield return new WaitForSeconds(f);
      _actor.gameObject.SetActive(false);
    }
  }
}