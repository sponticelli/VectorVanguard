using UnityEngine;
using VectorVanguard.Actors;
using VectorVanguard.Attributes;

namespace VectorVanguard.Levels
{
  [ScriptExecutionOrder(-500)]
  public class LevelManager : MonoBehaviour
  {
    [SerializeField] private ActiveActorsSO _activeActors;
    [SerializeField] private Camera _gameCamera;
    [SerializeField] private Actor _player;
    
    
    private void Awake()
    {
      _activeActors.Reset();
      _activeActors.MainCamera = _gameCamera;
      _activeActors.Player = _player;
    }
  }
}