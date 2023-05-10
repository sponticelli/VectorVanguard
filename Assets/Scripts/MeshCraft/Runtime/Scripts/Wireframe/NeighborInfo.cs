namespace LiteNinja.MeshCraft
{
  public class NeighborInfo
  {
    public int edgeIndex;
    public readonly int connect0To;
    public readonly int connect1To;
    public readonly int connect2To;
    public float Dot;
    public float Angle;
    public float Parallel;
    public float Area;

    public NeighborInfo(int index, int connect0To, int connect1To, int connect2To)
    {
      edgeIndex = index;
      this.connect0To = connect0To;
      this.connect1To = connect1To;
      this.connect2To = connect2To;
    }

    public float Weight() => (float) ((Dot + (double) Angle + (double) Parallel + (double) Area) / 4.0);
  }
}