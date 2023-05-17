using LiteNinja.PathMaster._2D;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.PathMaster.Editors
{
  [CustomEditor(typeof(MonoCenteredPathMeshBuilder))]
  public class MonoCenteredPathMeshBuilderEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var meshBuilder = (MonoCenteredPathMeshBuilder)target;

      GUILayout.Space(10f);

      if (GUILayout.Button("Generate Mesh"))
      {
        meshBuilder.Generate();
      }
    }
  }
}