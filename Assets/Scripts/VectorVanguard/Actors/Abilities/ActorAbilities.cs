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
  }
}