using UnityEditor;
using UnityEngine;
using Vektor.MeshFilters;

namespace Vektor.Editors
{
  [CustomEditor(typeof(VectorMeshFilter))]
  public class VectorMeshFilterEditor : Editor
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