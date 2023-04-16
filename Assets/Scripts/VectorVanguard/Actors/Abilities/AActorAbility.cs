using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public abstract class AActorAbility : MonoBehaviour
  {
    [Header("Permissions")]
    private bool _canExecute = true;
    
    
    public bool CanExecute
    {
      get => _canExecute;
      set => _canExecute = value;
    }

    
    protected bool _initialized;
    protected Actor _actor;


    public virtual void Initialization(Actor actor)
    {
      _actor = actor;
      OnInitialization();
      _initialized = true;
    }


    public virtual void EarlyExecute()
    {
      
    }
    public virtual void Execute()
    {
      
    }
    
    public virtual void LateExecute()
    {
      
    }
    

    protected virtual void OnInitialization() { }
    
  }
}