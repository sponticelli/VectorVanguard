namespace LiteNinja.MeshCraft
{
  /// <summary>
  /// Represents a set of three integer values that describe the barycentric coordinates of a point in a triangle.
  /// </summary>
  public class BarycentricCoords
  {
    /// <summary>
    /// The barycentric coordinate.
    /// </summary>
    public int x, y, z;

    /// <summary>
    /// Initializes a new instance of the BarycentricCoords class with the specified integer values.
    /// </summary>
    public BarycentricCoords(int xValue, int yValue, int zValue)
    {
      x = xValue;
      y = yValue;
      z = zValue;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current BarycentricCoords object.
    /// </summary>
    /// <param name="coords">The object to compare with the current BarycentricCoords object.</param>
    /// <returns>true if the specified object is equal to the current BarycentricCoords object; otherwise, false.</returns>
    public bool Equals(BarycentricCoords coords) => coords != null && x == coords.x && y == coords.y && z == coords.z;

    /// <summary>
    /// Serves as a hash function for the BarycentricCoords object.
    /// </summary>
    /// <returns>A hash code for the current BarycentricCoords object.</returns>
    public override int GetHashCode() => ((x * 397 ^ y) * 397 ^ z) * 397;
  }

}