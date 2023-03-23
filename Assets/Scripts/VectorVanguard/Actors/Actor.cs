using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using VectorVanguard.Actors.Abilities;

namespace VectorVanguard.Actors
{
  public class Actor : MonoBehaviour
  {
    [SerializeField]
    private AActorInput _input;
    
    [SerializeField]
    private ActorPhysics _physics;

    [SerializeField]
    private AActorAbility[] _abilities;
    
    
    public AActorAbility[] Abilities => _abilities;
    public AActorInput Input => _input;

    private void Awake()
    {
      Initialization();
    }
    
    private void Initialization()
    {
      _physics.Initialization(this);
      foreach (var ability in _abilities)
      {
        ability.Initialization(this);
      }
    }

    private void Update()
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