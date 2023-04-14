using System.Collections.Generic;
using System.Linq;

namespace VectorVanguard.Utils
{
  public static class EntityTypeExtensions
  {
    public static IEnumerable<EntityType> GetFlags(this EntityType entityType)
    {
      var types = EntityTypeUtils.GetTypes();
      return entityType == EntityType.None ? 
        types.Where(type => type == EntityType.None) : 
        types.Where(type => entityType.HasFlag(type));
    }
    
    public static bool IsComposite(this EntityType entityType)
    {
      var remaining = entityType;
      var types = EntityTypeUtils.GetTypes();
      return types.All(value => entityType != value);
    }
  }
}