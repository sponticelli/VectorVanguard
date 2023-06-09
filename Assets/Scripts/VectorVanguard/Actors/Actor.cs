using System;
using UnityEngine;
using VectorVanguard.Actors.Abilities;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors
{
  [Serializable]  
  public class Actor : MonoBehaviour, IFactionable
  {
    [SerializeField] protected EntityFaction _faction;
    [SerializeField] protected EntityType _entityType;
    
    [SerializeField] private AActorInput _input;

    [SerializeField] private AActorPhysics _physics;

    [SerializeField] private ActorAbilities _actorAbilities;

    public ActorAbilities ActorAbilities => _actorAbilities;
    public AActorPhysics Physics => _physics;

    public AActorInput Input => _input;
    
    public EntityFaction Faction
    {
      get => _faction;
      set => _faction = value;
    }
    
    public EntityType EntityType
    {
      get => _entityType;
      set => _entityType = value;
    }


    private void OnEnable()
    {
      Initialization();
    }

    protected virtual void Initialization()
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