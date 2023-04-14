using System;
using System.Collections.Generic;
using UnityEngine;
using VectorVanguard.Actors;
using VectorVanguard.Pools;
using VectorVanguard.Utils;

namespace VectorVanguard.Levels
{
  /// <summary>
  /// Manage a list of OutOfScreenPointer for the actor of a given EntityType and/or EntityFaction.
  /// </summary>
  public class OutOfScreenManager : MonoBehaviour
  {
    [SerializeField] private ActiveActorsSO _activeActors;
    [SerializeField] private EntityFaction _entityFactionMask;
    [SerializeField] private EntityType _entityTypeMask;
    [SerializeField] private APool _outOfScreenPointerPool;
    
    private Dictionary<int, OutOfScreenPointer> _outOfScreenPointers;

    private void Awake()
    {
      _outOfScreenPointerPool.Initialization();
      _outOfScreenPointers = new Dictionary<int, OutOfScreenPointer>();
    }

    private void OnEnable()
    {
      _activeActors.OnActorAdded.AddListener(OnActorAdded);
      _activeActors.OnActorRemoved.AddListener(OnActorRemoved);
    }
    
    private void OnDisable()
    {
      _activeActors.OnActorAdded.RemoveListener(OnActorAdded);
      _activeActors.OnActorRemoved.RemoveListener(OnActorRemoved);
    }

    private void OnActorRemoved(Actor actor)
    {
      var id = actor.GetInstanceID();
      if (_outOfScreenPointers.ContainsKey(id))
      {
        _outOfScreenPointers[id].gameObject.SetActive(false);
        _outOfScreenPointers.Remove(id);
      }
    }

    private void OnActorAdded(Actor actor)
    {
      var id = actor.GetInstanceID();
      //Check if the actor is of the right faction and type.
      var faction = actor.Faction;
      var type = actor.EntityType;
      
      if ((faction & _entityFactionMask) == 0 || (type & _entityTypeMask) == 0)
      {
        return;
      }

      if (_outOfScreenPointers.ContainsKey(id))
      {
        _outOfScreenPointers[id].gameObject.SetActive(true);
      }
      else
      {
        var go = _outOfScreenPointerPool.GetObject(Vector3.zero, Quaternion.identity);
        var outOfScreenPointer = go.GetComponent<OutOfScreenPointer>();
        _outOfScreenPointers.Add(id, outOfScreenPointer);
      }
      _outOfScreenPointers[id].Initialization(actor, _activeActors.Player, _activeActors.MainCamera);
    }
  }
}