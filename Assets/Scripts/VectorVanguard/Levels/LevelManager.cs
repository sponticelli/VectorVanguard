using UnityEngine;
using VectorVanguard.Attributes;

namespace VectorVanguard.Levels
{
  [ScriptExecutionOrder(-500)]
  public class LevelManager : MonoBehaviour
  {
    [SerializeField] private ActiveActorsSO _activeActors;
    
    private void Awake()
    {
      _activeActors.Reset();
    }
  }
}