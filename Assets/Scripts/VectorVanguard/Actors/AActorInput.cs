using System;
using UnityEngine;
using VectorVanguard.Inputs;

namespace VectorVanguard.Actors
{
  public abstract  class AActorInput : MonoBehaviour
  {
    public abstract Vector2 GetMovementDirection();
    public abstract IInputController.ButtonState GetState(string actionID);
    public abstract bool IsDown(string actionID);
    public abstract bool IsPressed(string actionID);
    public abstract bool IsUp(string actionID);
    
  }
}