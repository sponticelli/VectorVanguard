using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vektor.MeshFilters;

namespace Vektor.Editors
{
  [CustomEditor(typeof(VectorClosedMeshFilter))]
  public class VectorClosedMeshFilterEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      if (GUILayout.Button("Generate"))
      {
        ((VectorClosedMeshFilter) target).Generate(true);
      }
    }
  }
}