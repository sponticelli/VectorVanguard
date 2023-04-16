using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VectorVanguard.Actors;

namespace VectorVanguard.Levels
{
  /// <summary>
  /// A Scriptable Object that holds all the active actors in the scene.
  /// </summary>
  [CreateAssetMenu(fileName = "ActiveActorsSO", menuName = "VectorVanguard/SO/Create Active Actors List",
    order = 0)]
  public class ActiveActorsSO : ScriptableObject
  {
    private List<Actor> _actors;
    private Camera _mainCamera;
    private Actor _player;

    public Camera MainCamera
    {
      get => _mainCamera;
      set => _mainCamera = value;
    }
    
    public Actor Player
    {
      get => _player;
      set => _player = value;
    }
    
    public UnityEvent<Actor> OnActorAdded;
    public UnityEvent<Actor> OnActorRemoved;

    public void Reset()
    {
      if (_actors == null)
      {
        _actors = new List<Actor>();
      } 
      else
      {
        _actors.Clear();
      }
      
      _mainCamera = Camera.main;
    }

    public void Add(Actor actor)
    {
      _actors.Add(actor);
      OnActorAdded?.Invoke(actor);
    }

    public void Remove(Actor actor)
    {
      _actors.Remove(actor);
      OnActorRemoved?.Invoke(actor);
    }
    
  }
}