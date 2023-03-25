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
    private ActorAbilities _actorAbilities;
    
    public ActorAbilities ActorAbilities => _actorAbilities;
    
    public AActorInput Input => _input;

    private void Awake()
    {
      Initialization();
    }
    
    private void Initialization()
    {
      _physics.Initialization(this);
      _actorAbilities.Initialization(this);
    }

    private void Update()
    {
      _actorAbilities.Execute();
    }
    

  }
}