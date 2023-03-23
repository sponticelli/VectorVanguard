using System;
using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Inputs
{
  public abstract class AInputController : MonoBehaviour, IInputController
  {
    public event UnityAction<Vector2> OnMovement;
    public event Action<string, IInputController.ButtonState> OnButton;
    
    
    protected void InvokeOnMovementDirection(Vector2 direction)
    {
      OnMovement?.Invoke(direction);
    }
    
    protected void InvokeOnButton(string actionID, IInputController.ButtonState state)
    {
      OnButton?.Invoke(actionID, state);
    }
  }
}