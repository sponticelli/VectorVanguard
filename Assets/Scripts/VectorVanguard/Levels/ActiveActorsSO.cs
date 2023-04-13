using System.Collections.Generic;
using UnityEngine;
using VectorVanguard.Actors;

namespace VectorVanguard.Levels
{
  
  /// <summary>
  /// A Scriptable Object that holds all the active actors in the scene.
  /// </summary>
  [CreateAssetMenu(fileName = "ActiveActorsSO", menuName = "LiteNinja/VectorVanguard/SO/Create Active Actors", order = 0)]
  public class ActiveActorsSO : ScriptableObject
  {
    [SerializeField] private Actor _player;
    [SerializeField] private List<Actor> _asteroids;


    public Actor Player
    {
      get => _player;
      set => _player = value;
    }
    
    public List<Actor> Asteroids
    {
      get => _asteroids;
      set => _asteroids = value;
    }
    
  }
  
  
}