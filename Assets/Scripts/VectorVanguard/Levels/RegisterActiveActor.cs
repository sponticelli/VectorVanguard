using UnityEngine;
using VectorVanguard.Actors;

namespace VectorVanguard.Levels
{
  [RequireComponent(typeof(Actor))]
  public class RegisterActiveActor : MonoBehaviour
  {
    [SerializeField] private ActiveActorsSO _activeActorsSO;
    [SerializeField] private Actor _actor;
    
    private void Awake()
    {
      _actor = GetComponent<Actor>();
    }

    private void OnEnable()
    {
      _activeActorsSO.Add(_actor);
    }

    private void OnDisable()
    {
      _activeActorsSO.Remove(_actor);
    }
  }
}