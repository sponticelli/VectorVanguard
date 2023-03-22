using UnityEditor;
using UnityEngine;
using Vektor.Meshes;
using Vektor.MeshFilters;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  public class LineVectorCreator : ConnectedVectorCreator
  {
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.one;


    [MenuItem("LiteNinja/Vektor/Line Creator")]
    public static void ShowWindow()
    {
      GetWindow<LineVectorCreator>("Line Creator");
    }

    private void OnGUI()
    {
      GUILayout.Label("Line Settings", EditorStyles.boldLabel);
      _startPoint = EditorGUILayout.Vector3Field("Start Point", _startPoint);
      _endPoint = EditorGUILayout.Vector3Field("End Point", _endPoint);
      _lineWidth = EditorGUILayout.FloatField("Line Width", _lineWidth);
      _joinType = (VectorMesh.JoinType)EditorGUILayout.EnumPopup("Join Type", _joinType);
      _material = (Material)EditorGUILayout.ObjectField("Material", _material, typeof(Material), false);

      if (GUILayout.Button("Create Line"))
      {
        CreateConnectedVector(new[] {_startPoint, _endPoint}, "Line");
      }
    }
  }
}