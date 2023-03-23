using System.Collections.Generic;
using UnityEngine;
using VectorVanguard.Inputs;

namespace VectorVanguard.Actors
{
  public class PlayerInput : AActorInput
  {
    
    [SerializeField]
    private AInputController _inputController;
    
    private Vector2 _movementDirection;
    private Dictionary<string, IInputController.ButtonState> _buttonStates;
    
    private void Awake()
    {
      _buttonStates = new Dictionary<string, IInputController.ButtonState>();
    }

    private void OnEnable()
    {
      _inputController.OnMovement += SetMovementDirection;
      _inputController.OnButton += SetButtonState;
      _movementDirection = Vector2.zero;
    }
    
    private void OnDisable()
    {
      _inputController.OnMovement -= SetMovementDirection;
      _inputController.OnButton -= SetButtonState;
      _buttonStates.Clear();
    }


    public override Vector2 GetMovementDirection()
    {
      return _movementDirection;
    }

    public override IInputController.ButtonState GetState(string actionID)
    {
      return !_buttonStates.ContainsKey(actionID) ? IInputController.ButtonState.None : _buttonStates[actionID];
    }
    
    public override bool IsDown(string actionID)
    {
      return GetState(actionID) == IInputController.ButtonState.Down;
    }
    
    public override bool IsPressed(string actionID)
    {
      return GetState(actionID) == IInputController.ButtonState.Pressed;
    }
    
    public override bool IsUp(string actionID)
    {
      return GetState(actionID) == IInputController.ButtonState.Up;
    }
    
    private void SetMovementDirection(Vector2 movement)
    {
      _movementDirection = movement;
    }
    
    private void SetButtonState(string actionID, IInputController.ButtonState state)
    {
      _buttonStates[actionID] = state;
    }
  }
}