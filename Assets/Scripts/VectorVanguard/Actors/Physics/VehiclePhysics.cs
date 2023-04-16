using UnityEngine;

namespace VectorVanguard.Actors.Vehicles
{
  /// <summary>
  /// It simulates the physics of vehicle with tires.
  /// </summary>
  public class VehiclePhysics : AActorPhysics
  {
    [Header("Components")] 
    [SerializeField] private Rigidbody2D _rigidbody2D;
    
   
    [Header("Vehicle settings")]
    [SerializeField] private float _driftFactor = 0.95f;
    [SerializeField] private float _accelerationFactor = 30.0f;
    [SerializeField] private float _turnFactor = 3.5f;
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private float _linearDrag = 3f;
    [SerializeField] private bool _canGoInReverse;

    
    private float _accelerationInput;
    private float _steeringInput;
    private float _rotationAngle;
    private float _forwardVelocity;
    
    private Vector2 _thrustForce;
    
    
    private float _externalRotationForce;
    private Vector3 _externalForce = Vector2.zero;

    public override void Initialization(Actor actor)
    {
      _actor = actor;
      if (_rigidbody2D == null)
      {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
          Debug.LogError("Rigidbody2D is missing on the actor.");
        }
      }
      _rotationAngle =  _actor.transform.rotation.eulerAngles.z;
    }
    
    private void FixedUpdate()
    {
      Thrust();
      Drift();
      Steer();
      ApplyExternalForces();
      ResetInput();
      ResetExternalForces();
    }
    
    private void ApplyExternalForces()
    {
      _rigidbody2D.AddForce(_externalForce, ForceMode2D.Force);
      _rigidbody2D.AddTorque(_externalRotationForce, ForceMode2D.Force);
    }

    private void ResetInput()
    {
      _accelerationInput = 0;
      _steeringInput = 0;
    }

    private void ResetExternalForces()
    {
      _externalForce = Vector3.zero;
      _externalRotationForce = 0;
    }


    public override void AddRotationForce(float rotation)
    {
      _steeringInput = rotation;
    }

    public override void AddForce(Vector3 force)
    {
      _accelerationInput = force.y;
      if (!_canGoInReverse && _accelerationInput < 0)
        _accelerationInput = 0;
    }

    public override void AddExternalRotationForce(float rotation)
    {
      _externalRotationForce += rotation;
    }

    public override void AddExternalForce(Vector3 force)
    {
      _externalForce += force;
    }

    public override float GetSpeed()
    {
      return _rigidbody2D.velocity.magnitude;
    }

    private void Thrust()
    {
      //Apply drag if the vehicle is not accelerating so that it is slowing down
      _rigidbody2D.drag = (_accelerationInput == 0) ? Mathf.Lerp(_rigidbody2D.drag, _linearDrag, Time.fixedDeltaTime * 3) : 0;

      if (LimitVelocity()) return;

      //Create a force for the engine
      _thrustForce = transform.up * (_accelerationInput * _accelerationFactor);

      //Apply force and pushes the car forward
      _rigidbody2D.AddForce(_thrustForce, ForceMode2D.Force);
    }

    private bool LimitVelocity()
    {
      //Caculate how much "forward" we are going in terms of the direction of our velocity
      _forwardVelocity = Vector2.Dot(transform.up, _rigidbody2D.velocity);

      //Limit so we cannot go faster than the max speed in the "forward" direction
      if (_forwardVelocity > _maxSpeed && _accelerationInput > 0) return true;

      //Limit so we cannot go faster than the 50% of max speed in the "reverse" direction
      if (_forwardVelocity < -_maxSpeed * 0.5f && _accelerationInput < 0) return true;

      //Limit so we cannot go faster in any direction while accelerating
      if (_rigidbody2D.velocity.sqrMagnitude > _maxSpeed * _maxSpeed && _accelerationInput > 0) return true;
      
      return false;
    }

    private void Steer()
    {
      _rotationAngle += _steeringInput * _turnFactor;
      _rigidbody2D.MoveRotation(_rotationAngle);
    }

    private void Drift()
    {
      //Get forward and right velocity of the car
      Vector2 forwardVelocity = transform.up * Vector2.Dot(_rigidbody2D.velocity, transform.up);
      Vector2 rightVelocity = transform.right * Vector2.Dot(_rigidbody2D.velocity, transform.right);

      //reduce the side velocity by the drift factor
      _rigidbody2D.velocity = forwardVelocity + rightVelocity * _driftFactor;
    }
  }
}