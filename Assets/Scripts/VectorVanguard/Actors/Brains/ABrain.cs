using UnityEngine;

namespace VectorVanguard.Actors.Brains
{
  [RequireComponent(typeof(Actor))]
  public abstract class ABrain : AActorInput
  {
    protected Actor _actor;

    protected virtual void Awake()
    {
      _actor = GetComponent<Actor>();
      Initialization(_actor);
    }
    
    protected virtual void Update()
    {
      Think();
    }

    protected abstract void Initialization(Actor actor);
    protected abstract void Think();
  }
}