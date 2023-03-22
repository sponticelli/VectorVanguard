using UnityEditor;
using UnityEngine;
using Vektor.Meshes;

namespace Vektor.Editors
{
  public class EllipseVectorCreator : ConnectedVectorCreator
  {
    private float _radiusX = 2f;
    private float _radiusY = 1f;
    private int _numSegments = 20;

    [MenuItem("LiteNinja/Vektor/Ellipse Creator")]
    public static void ShowWindow()
    {
      GetWindow<EllipseVectorCreator>("Ellipse Creator");
    }

    private void OnGUI()
    {
      GUILayout.Label("Ellipse Settings", EditorStyles.boldLabel);
      _radiusX = EditorGUILayout.FloatField("Radius X", _radiusX);
      _radiusY = EditorGUILayout.FloatField("Radius Y", _radiusY);
      _numSegments = EditorGUILayout.IntField("Number of Segments", _numSegments);
      _lineWidth = EditorGUILayout.FloatField("Line Width", _lineWidth);
      _joinType = (VectorMesh.JoinType)EditorGUILayout.EnumPopup("Join Type", _joinType);
      _material = (Material)EditorGUILayout.ObjectField("Material", _material, typeof(Material), false);
      _isClosed = EditorGUILayout.Toggle("Is Closed", _isClosed);

      if (GUILayout.Button("Create Ellipse"))
      {
        CreateEllipse();
      }
    }

    private void CreateEllipse()
    {
      if (_numSegments < 3)
      {
        Debug.LogError("Number of segments must be at least 3");
        return;
      }

      var points = new Vector3[_numSegments];
      var angleIncrement = 2f * Mathf.PI / _numSegments;

      for (var i = 0; i < _numSegments; i++)
      {
        var angle = i * angleIncrement;
        points[i] = new Vector3(_radiusX * Mathf.Cos(angle), _radiusY * Mathf.Sin(angle), 0);
      }

      CreateConnectedVector(points, GetEllipseName());
    }

    public string GetEllipseName()
    {
      return $"Ellipse_{_radiusX}x{_radiusY}";
    }
  }
}