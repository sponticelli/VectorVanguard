namespace LiteNinja.MeshCraft.Wireframe
{
  /// <summary>
  /// The TriangleNeighborRelationship class represents information about a neighboring triangle.
  /// </summary>
  public class TriangleNeighborRelationship
  {
    // Index of the shared edge between the current triangle and its neighbor.
    public int edgeIndex;
    
    // Indices representing the connected vertices between the current triangle and its neighbor.
    public readonly int connect0To;
    public readonly int connect1To;
    public readonly int connect2To;
    
    // Dot product of the normals of the current triangle and its neighbor.
    public float Dot;
    
    // Refined angle between the current triangle and its neighbor, based on their edge and top vertices.
    public float Angle;
    
    // A coefficient representing the parallelism between the current triangle and its neighbor.
    public float Parallel;
    
    // A coefficient representing the area ratio between the current triangle and its neighbor.
    public float Area;

    // Method to calculate the weight for the current neighbor, based on the Dot, Angle, Parallel, and Area properties.
    public TriangleNeighborRelationship(int index, int connect0To, int connect1To, int connect2To)
    {
      edgeIndex = index;
      this.connect0To = connect0To;
      this.connect1To = connect1To;
      this.connect2To = connect2To;
    }

    public float Weight() => (float) ((Dot + (double) Angle + (double) Parallel + (double) Area) / 4.0);
  }
}