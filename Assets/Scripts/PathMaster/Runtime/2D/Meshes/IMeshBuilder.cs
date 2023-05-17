using Unity.VisualScripting;
using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  public interface IMeshBuilder
  {
    public Mesh BuildMesh(Path2D path);
  }
}