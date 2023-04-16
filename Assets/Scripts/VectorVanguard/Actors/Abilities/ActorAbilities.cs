using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorAbilities : MonoBehaviour
  {
    [SerializeField] private AActorAbility[] _abilities;
    
    
    public UnityEvent<AActorAbility> OnAbilityAdded;
    public UnityEvent<AActorAbility> OnAbilityRemoved;
    
    public void Initialization(Actor actor)
    {
      foreach (var ability in _abilities)
      {
        ability.Initialization(actor);
        OnAbilityAdded?.Invoke(ability);
      }
    }

    public void Execute()
    {
      foreach (var ability in _abilities)
      {
        ability.EarlyExecute();
      }

      foreach (var ability in _abilities)
      {
        ability.Execute();
      }
      
      foreach (var ability in _abilities)
      {
        ability.LateExecute();
      }
    }
    
    /// <summary>
    /// Returns the first ability of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetAbility<T>() where T : AActorAbility
    {
      return _abilities.OfType<T>().FirstOrDefault();
    }
    
    /// <summary>
    /// Add an ability to the actor
    /// </summary>
    /// <param name="ability"></param>
    public void AddAbility(AActorAbility ability)
    {
      var abilities = _abilities.ToList();
      abilities.Add(ability);
      _abilities = abilities.ToArray();
      OnAbilityAdded?.Invoke(ability);
    }
    
    /// <summary>
    /// Remove an ability from the actor
    /// </summary>
    /// <param name="ability"></param>
    public void RemoveAbility(AActorAbility ability)
    {
      var abilities = _abilities.ToList();
      if (abilities.Contains(ability) == false) return;
      abilities.Remove(ability);
      _abilities = abilities.ToArray();
      OnAbilityRemoved?.Invoke(ability);
    }
  }
}