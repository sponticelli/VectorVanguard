using UnityEditor;
using UnityEngine;
using Vektor.MeshFilters;

namespace Vektor.Editors
{
  [CustomEditor(typeof(AVectorMeshFilter))]
  public class AVectorMeshFilterEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      if (GUILayout.Button("Generate"))
      {
        ((AVectorMeshFilter) target).Generate(true);
      }
    }
  }
}