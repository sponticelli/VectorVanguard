using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VectorVanguard.Actors;
using VectorVanguard.Utils;

namespace VectorVanguard.Levels
{

  /// <summary>
  /// A Scriptable Object that holds all the active actors in the scene.
  /// </summary>
  [CreateAssetMenu(fileName = "ActiveActorsSO", menuName = "VectorVanguard/SO/Create Active Actors List",
    order = 0)]
  public class ActiveActorsSO : ScriptableObject
  {
    private Dictionary<EntityType, List<Actor>> _actors;
    

    public void Reset()
    {
      _actors = new Dictionary<EntityType, List<Actor>>();
    }

    public void Add(Actor actor)
    {
      var entityType = actor.EntityType;
      if (!_actors.ContainsKey(entityType))
      {
        _actors.Add(entityType, new List<Actor>());
      }
      _actors[entityType].Add(actor);
    }

    public void Remove(Actor actor)
    {
      var entityType = actor.EntityType;
      if (!_actors.ContainsKey(entityType))
      {
        return;
      }
      _actors[entityType].Remove(actor);
    }
    
    public IEnumerable<Actor> OfType(EntityType entityType)
    {
      return !_actors.ContainsKey(entityType) ? Enumerable.Empty<Actor>() : _actors[entityType];
    }
    
    public IEnumerable<Actor> OfTypeAndFaction(EntityType entityType, EntityFaction faction)
    {
      return !_actors.ContainsKey(entityType) ? Enumerable.Empty<Actor>() : _actors[entityType].Where(x => x.Faction == faction);
    }
    
    public IEnumerable<Actor> OfFaction(EntityFaction faction)
    {
      return _actors.SelectMany(x => x.Value).Where(x => x.Faction == faction);
    }
    
    public Actor FirstOfType(EntityType entityType)
    {
      return !_actors.ContainsKey(entityType) ? null : _actors[entityType].FirstOrDefault();
    }
    
    public Actor FirstOfTypeAndFaction(EntityType entityType, EntityFaction faction)
    {
      return !_actors.ContainsKey(entityType) ? null : _actors[entityType].FirstOrDefault(x => x.Faction == faction);
    }
    
    public Actor FirstOfFaction(EntityFaction faction)
    {
      return _actors.SelectMany(x => x.Value).FirstOrDefault(x => x.Faction == faction);
    }
    
    public IEnumerable<Actor> All()
    {
      return _actors.SelectMany(x => x.Value);
    }
    
  }
}