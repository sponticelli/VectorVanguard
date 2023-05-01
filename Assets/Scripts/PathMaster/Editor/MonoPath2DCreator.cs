using UnityEditor;
using UnityEngine;

namespace LiteNinja.PathMaster.Editors
{
  public static class MonoPath2DCreator
  {
    [MenuItem("LiteNinja/PathMaster/MonoPath2D")]
    private static void CreateNewPath2D()
    {
      var go = new GameObject("MonoPath2D");
      var monoPath2D = go.AddComponent<MonoPath2D>();
      if (Selection.transforms.Length <= 0) return;
      go.transform.parent = Selection.transforms[0];
      monoPath2D.Path = new Path2D(Selection.transforms[0].position, monoPath2D.DefaultControlType);
      monoPath2D.Initialize();
    }
  }
}