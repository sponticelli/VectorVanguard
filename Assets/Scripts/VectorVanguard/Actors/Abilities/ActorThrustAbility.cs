using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorThrustAbility : AActorAbility
  {
    [SerializeField] private float _thrustPower = 1;
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private float _linearDecayRate = 0.7f;

    [SerializeField] private bool _useMovementDirection = true;
    [SerializeField] private string _thrustActionID = "THRUST";

    public UnityEvent OnThrustStart;
    public UnityEvent OnThrustEnd;
    public UnityEvent OnThrust;
    
    
    
    private bool _isThrusting;
    private bool _wasThrusting;
    private float _linearVelocity;
    private AActorPhysics _physics;

    private Vector3 _linearForce;

    

    protected override void OnInitialization()
    {
      base.OnInitialization();
      _physics = _actor.Physics;
      _isThrusting = false;
      _wasThrusting = false;
      _linearForce = Vector2.zero;
    }

    public override void EarlyExecute()
    {
      base.EarlyExecute();
      _wasThrusting = _isThrusting;
      if (_useMovementDirection)
      {
        _isThrusting = _actor.Input.GetMovementDirection().y != 0;
      }
      else
      {
        _isThrusting = _actor.Input.IsPressed(_thrustActionID);
      }

      InvokeEvents();
    }

    private void InvokeEvents()
    {
      switch (_isThrusting)
      {
        case true when !_wasThrusting:
          OnThrustStart?.Invoke();
          break;
        case false when _wasThrusting:
          OnThrustEnd?.Invoke();
          break;
        case true when _wasThrusting:
          OnThrust?.Invoke();
          break;
      }
    }

    public override void Execute()
    {
      base.Execute();

      if (_isThrusting)
      {
        _linearForce += _actor.transform.up * _thrustPower; // Apply thrust force based on deltaTime
      }
      _linearForce *= _linearDecayRate; // Apply linear decay to the force
      _linearForce = Vector3.ClampMagnitude(_linearForce, _maxSpeed); // Clamp the force to the max speed
      //if _linearForce.magnitude < epsilon, set to zero
      _linearForce = _linearForce.magnitude < 0.0001f ? Vector3.zero : _linearForce;
      _physics.AddForce(_linearForce);

      // Draw a line to show the direction of thrust
      Debug.DrawLine(_actor.transform.position, _actor.transform.position + (Vector3)_linearForce, Color.green);
    }
  }
}