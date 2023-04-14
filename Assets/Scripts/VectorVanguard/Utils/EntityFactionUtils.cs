using System;

namespace VectorVanguard.Utils
{
  public static class EntityFactionUtils
  {
    public static EntityFaction[] GetFactions()
    {
      return (EntityFaction[])Enum.GetValues(typeof(EntityFaction));
    }
  }
}