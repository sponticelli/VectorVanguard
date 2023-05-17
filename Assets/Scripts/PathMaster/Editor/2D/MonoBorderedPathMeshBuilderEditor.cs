using LiteNinja.PathMaster._2D;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.PathMaster.Editors
{
  [CustomEditor(typeof(MonoBorderedPathMeshBuilder))]
  public class MonoBorderedPathMeshBuilderEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      var meshBuilder = (MonoBorderedPathMeshBuilder)target;

      GUILayout.Space(10f);

      if (GUILayout.Button("Generate Mesh"))
      {
        meshBuilder.Generate();
      }
    }
  }
}