using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VectorVanguard.Utils
{
  public static class EntityFactionExtensions
  {

    public static void SetFaction(this IFactionable factionable, EntityFaction faction)
    {
      factionable.Faction = faction;
    }
    
    public static void SetFaction(this GameObject gameObject, EntityFaction faction)
    {
      var factionable = gameObject.GetComponent<IFactionable>();
      factionable?.SetFaction(faction);
    }
    
    public static EntityFaction GetFaction(this IFactionable factionable)
    {
      return factionable.Faction;
    }
    
    public static EntityFaction GetFaction(this GameObject gameObject)
    {
      var factionable = gameObject.GetComponent<IFactionable>();
      return factionable?.GetFaction() ?? EntityFaction.None;
    }

    public static IEnumerable<EntityFaction> GetFlags(this EntityFaction entityFaction)
    {
      var factions = EntityFactionUtils.GetFactions();
      return entityFaction == EntityFaction.None ? 
        factions.Where(type => type == EntityFaction.None) : 
        factions.Where(type => entityFaction.HasFlag(type));
    }
    
    public static bool IsComposite(this EntityFaction entityFaction)
    {
      var remaining = entityFaction;
      var factions = EntityFactionUtils.GetFactions();
      return factions.All(faction => entityFaction != faction);
    }
    
    
  }
}