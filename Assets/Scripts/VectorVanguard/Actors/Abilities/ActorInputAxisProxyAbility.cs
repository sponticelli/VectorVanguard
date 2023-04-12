using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorInputAxisProxyAbility : AActorAbility
  {
    [SerializeField] private bool _invertXDirection = true;
    
    
    [Header("X Axis Events")]
    public UnityEvent OnPositiveXAxisStart;
    public UnityEvent OnPositiveXAxisEnd;
    public UnityEvent OnPositiveXAxis;
    public UnityEvent OnNegativeXAxisStart;
    public UnityEvent OnNegativeXAxisEnd;
    public UnityEvent OnNegativeXAxis;
    
    [Header("Y Axis Events")]
    public UnityEvent OnPositiveYAxisStart;
    public UnityEvent OnPositiveYAxisEnd;
    public UnityEvent OnPositiveYAxis;
    public UnityEvent OnNegativeYAxisStart;
    public UnityEvent OnNegativeYAxisEnd;
    public UnityEvent OnNegativeYAxis;
    
    
    private AActorPhysics _physics;
    private float _directionMultiplier = 1;
    
    private AxisState _positiveXAxisState;
    private AxisState _negativeXAxisState;
    private AxisState _positiveYAxisState;
    private AxisState _negativeYAxisState;


    private struct AxisState
    {
      public bool isActivated { get; private set; }
      public bool wasActivated { get; private set; }
      
      public AxisState(bool isActivated = false, bool wasActivated = false)
      {
        this.isActivated = isActivated;
        this.wasActivated = wasActivated;
      }
      
      public void Update(bool isActivated)
      {
        wasActivated = this.isActivated;
        this.isActivated = isActivated;
      }
    }
    

    protected override void OnInitialization()
    {
      base.OnInitialization();
      _physics = _actor.Physics;
      _directionMultiplier = _invertXDirection ? -1 : 1;
      
      _positiveXAxisState = new AxisState();
      _negativeXAxisState = new AxisState();
      _positiveYAxisState = new AxisState();
      _negativeYAxisState = new AxisState();
    }
    
    public override void EarlyExecute()
    {
      base.EarlyExecute();
      var x = _directionMultiplier * _actor.Input.GetMovementDirection().x;
      var y = _actor.Input.GetMovementDirection().y;
      
      _positiveXAxisState.Update(x != 0 && x > 0);
      _negativeXAxisState.Update(x != 0 && x < 0);
      _positiveYAxisState.Update(y != 0 && y > 0);
      _negativeYAxisState.Update(y != 0 && y < 0);
      
      if (x != 0) _physics.AddRotationForce(x);
      if (y != 0) _physics.AddForce(new Vector3(0, y, 0));
      
      InvokeEvents();
    }

    private void InvokeEvents()
    {
      CheckAxisState(_positiveXAxisState, OnPositiveXAxisStart, OnPositiveXAxisEnd, OnPositiveXAxis);
      CheckAxisState(_negativeXAxisState, OnNegativeXAxisStart, OnNegativeXAxisEnd, OnNegativeXAxis);
      CheckAxisState(_positiveYAxisState, OnPositiveYAxisStart, OnPositiveYAxisEnd, OnPositiveYAxis);
      CheckAxisState(_negativeYAxisState, OnNegativeYAxisStart, OnNegativeYAxisEnd, OnNegativeYAxis);
    }
    
    private void CheckAxisState(AxisState state, UnityEvent startEvent, UnityEvent endEvent, UnityEvent onGoingEvent)
    {
      switch (state.isActivated)
      {
        case true when !state.wasActivated:
          startEvent.Invoke();
          break;
        case false when state.wasActivated:
          endEvent.Invoke();
          break;
        case true when state.wasActivated:
          onGoingEvent.Invoke();
          break;
      }
    }
  }
}