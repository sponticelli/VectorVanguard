namespace LiteNinja.MeshCraft
{
  /// <summary>
  /// Defines the attributes of a mesh that should be taken into account when operating
  /// on the mesh (ie. merging, generating wireframe, etc.)
  /// </summary>
  public class MeshAttributes
  {
    public bool UseUV2;
    public bool UseUV3;
    public bool UseUV4;
    public bool UseNormals;
    public bool UseTangents;
    public bool UseColors;
    public bool UseSkin;

    public MeshAttributes() => UseUV2 = UseUV3 = UseUV4 = UseNormals = UseTangents = UseColors = UseSkin = true;
  }
}