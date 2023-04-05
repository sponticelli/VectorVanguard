using UnityEngine;
using VectorVanguard.Actors.Abilities;
using VectorVanguard.Inputs;

namespace VectorVanguard.Actors.Brains
{
  public  class AsteroidBrain : ABrain
  {
    [SerializeField] private float _minThrustForce = 0.1f;
    [SerializeField] private float _maxThrustForce = 1f;
    
    private Vector2 _rotationDirection;
    private Vector3 _thrusterDirection;
    
    protected override void Initialization(Actor actor)
    {
      _rotationDirection = new Vector2(Random.Range(-1f, 1f) > 0 ? 1 : -1, 0);
      
      _thrusterDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
      _thrusterDirection.Normalize();
      
      var autoThrustAbility = actor.ActorAbilities.GetAbility<ActorAutoThrustAbility>();
      if (autoThrustAbility) 
      {
        autoThrustAbility.ThrustDirection = _thrusterDirection;
        autoThrustAbility.ThrustForce = Random.Range(_minThrustForce, _maxThrustForce);
      }
      else
      {
        Debug.LogError("AsteroidBrain: Initialization: ActorAutoThrustAbility not found");
      }
    }
    
    protected override void Think()
    {
      // Do nothing - it is just a rock
    }
    
    
    #region Input
    public override Vector2 GetMovementDirection()
    {
      return _rotationDirection;
    }

    public override IInputController.ButtonState GetState(string actionID)
    {
      return IInputController.ButtonState.None;
    }

    public override bool IsDown(string actionID)
    {
      return false;
    }

    public override bool IsPressed(string actionID)
    {
      return false;
    }

    public override bool IsUp(string actionID)
    {
      return false;
    }
    #endregion
  }
}