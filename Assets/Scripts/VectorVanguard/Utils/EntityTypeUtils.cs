using System;
using System.Collections.Generic;

namespace VectorVanguard.Utils
{
  public static class EntityTypeUtils
  {
    public static IEnumerable<EntityType> GetTypes()
    {
      return (EntityType[])Enum.GetValues(typeof(EntityType));
    }
  }
}