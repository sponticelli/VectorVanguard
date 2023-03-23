using UnityEngine;

namespace VectorVanguard.Inputs
{
  public class MultiInputController : AInputController
  {
    [SerializeField]
    private AInputController[] _inputControllers;
    
    private void OnEnable()
    {
      foreach (var inputController in _inputControllers)
      {
        inputController.OnMovement += InvokeOnMovementDirection;
        inputController.OnButton += InvokeOnButton;
      }
    }
    
    private void OnDisable()
    {
      foreach (var inputController in _inputControllers)
      {
        inputController.OnMovement -= InvokeOnMovementDirection;
        inputController.OnButton -= InvokeOnButton;
      }
    }
  }
}