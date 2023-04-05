using System.Linq;
using UnityEngine;
using VectorVanguard.Actors.Abilities;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorAbilities : MonoBehaviour
  {
    [SerializeField] private AActorAbility[] _abilities;
    
    public void Initialization(Actor actor)
    {
      foreach (var ability in _abilities)
      {
        ability.Initialization(actor);
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
  }
}