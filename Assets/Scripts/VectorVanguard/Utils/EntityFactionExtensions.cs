using UnityEngine;

namespace VectorVanguard.Utils
{
  public static class EntityFactionExtensions
  {
    public static bool IsEnemy(this EntityFaction faction, EntityFaction otherFaction)
    {
      return faction == EntityFaction.Player && otherFaction == EntityFaction.Enemy ||
             faction == EntityFaction.Enemy && otherFaction == EntityFaction.Player;
    }
    
    public static bool IsFriendly(this EntityFaction faction, EntityFaction otherFaction)
    {
      return faction == EntityFaction.Player && otherFaction == EntityFaction.Player ||
             faction == EntityFaction.Enemy && otherFaction == EntityFaction.Enemy;
    }
    
    public static bool IsNeutral(this EntityFaction faction)
    {
      return faction == EntityFaction.Neutral;
    }
    
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
  }
}