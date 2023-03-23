using System;

using UnityEngine;
using UnityEngine.Events;


namespace VectorVanguard.Inputs
{
  public interface IInputController
  {
    event UnityAction<Vector2> OnMovement;
    event Action<string, ButtonState> OnButton;

    public enum ButtonState
    {
      None,
      Down,
      Pressed,
      Up
    }
  }
}