using System;
using System.Collections.Generic;
using UnityEngine;

namespace VectorVanguard.Inputs
{
  public class KeyboardInputController : AInputController
  {
    [SerializeField]
    private ButtonAction[] _buttonActions;

    private Vector2 _lastMovementDirection;

    private void Awake()
    {
      _lastMovementDirection = Vector2.zero;
    }
    

    private void Update()
    {
      InvokeOnMovementDirection(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

      foreach (var buttonAction in _buttonActions)
      {
        var key = buttonAction.Key;
        if (Input.GetKeyDown(key))
        {
          InvokeOnButton(buttonAction.ActionID, IInputController.ButtonState.Down);
          continue;
        }

        if (Input.GetKey(key))
        {
          InvokeOnButton(buttonAction.ActionID, IInputController.ButtonState.Pressed);
          continue;
        }

        if (Input.GetKeyUp(key))
        {
          InvokeOnButton(buttonAction.ActionID, IInputController.ButtonState.Up);
          continue;
        }

        InvokeOnButton(buttonAction.ActionID, IInputController.ButtonState.None);
      }

    }


    [Serializable]
    public class ButtonAction
    {
      public string ActionID;
      public KeyCode Key;
    }
  }
}