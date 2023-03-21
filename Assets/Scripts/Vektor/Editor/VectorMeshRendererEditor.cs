using UnityEditor;
using UnityEngine;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  [CustomEditor(typeof(VectorMeshRenderer))]
  public class VectorMeshRendererEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      
      var vectorMeshRenderer = (VectorMeshRenderer)target;
      ShowSetupButton(vectorMeshRenderer);
      ShowGenerateButton(vectorMeshRenderer);
    }

    private static void ShowGenerateButton(VectorMeshRenderer vectorMeshRenderer)
    {
      if (!GUILayout.Button("Generate")) return;
      vectorMeshRenderer.SetupComponents();
      vectorMeshRenderer.Generate(true);
    }

    private static void ShowSetupButton(VectorMeshRenderer vectorMeshRenderer)
    {
      if (vectorMeshRenderer.MeshFilter != null && vectorMeshRenderer.MeshRenderer != null) return;
      if (GUILayout.Button("Setup Components"))
      {
        vectorMeshRenderer.SetupComponents();
      }
    }
  }
}