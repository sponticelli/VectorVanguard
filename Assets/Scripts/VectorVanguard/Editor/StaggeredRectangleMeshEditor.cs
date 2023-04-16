using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using VectorVanguard.Utils.Meshes;

namespace VectorVanguard.Attributes
{
  [CustomEditor(typeof(StaggeredRectangleMesh))]
  public class StaggeredRectangleMeshEditor : Editor
  {
    private StaggeredRectangleMesh _controller;

    private void OnEnable()
    {
      _controller = target as StaggeredRectangleMesh;
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      GUILayout.Space(10);
      if (GUILayout.Button("Generate"))
      {
        Generate();
      }
    }

    private void Generate()
    {
      _controller.Init();
      _controller.Generate();
    }
  }
}