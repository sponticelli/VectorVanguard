using UnityEngine;

namespace VectorVanguard.Actors.Abilities
{
  public class ActorRadarAbility : AActorAbility
  {
    [SerializeField] private float _range = 10f;
    
    public float Range 
    {
      get => _range;
      set => _range = value;
    }
  }
}