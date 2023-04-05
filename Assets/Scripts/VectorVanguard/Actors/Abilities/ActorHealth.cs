using UnityEngine;
using UnityEngine.Events;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorHealth : AActorAbility, IDamageable
  {
    [SerializeField] private float _maxHealth;
    
    private float _currentHealth;


    public float CurrentHealth
    {
      get => _currentHealth;
      set => _currentHealth = value;
    }

    public float MaxHealth
    {
      get => _maxHealth;
      set => _maxHealth = value;
    }
    
    public UnityEvent OnDeath;
    
    public override void Initialization(Actor actor)
    {
      base.Initialization(actor);
      _currentHealth = _maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
      var _previousHealth = _currentHealth;
      _currentHealth -= damage;
      if (_currentHealth <= 0 && _previousHealth > 0)
      {
        _currentHealth = 0;
        OnDeath.Invoke();
      }
    }
  }
}