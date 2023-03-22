using UnityEditor;
using UnityEngine;
using Vektor.Meshes;
using Vektor.MeshFilters;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  public class PolygonVectorCreator : ConnectedVectorCreator
  {
    private float _radius = 1f;
    private int _numSides = 3;
    private float _angle = 0f;

    [MenuItem("LiteNinja/Vektor/Polygon Creator")]
    public static void ShowWindow()
    {
      GetWindow<PolygonVectorCreator>("Polygon Creator");
    }

    private void OnGUI()
    {
      GUILayout.Label("Inscribed Polygon Settings", EditorStyles.boldLabel);
      _radius = EditorGUILayout.FloatField("Radius", _radius);
      _numSides = EditorGUILayout.IntField("Number of Sides", _numSides);
      _angle = EditorGUILayout.IntSlider("Angle", (int)_angle, -90, 90);
      _lineWidth = EditorGUILayout.FloatField("Line Width", _lineWidth);
      _joinType = (VectorMesh.JoinType)EditorGUILayout.EnumPopup("Join Type", _joinType);
      _material = (Material)EditorGUILayout.ObjectField("Material", _material, typeof(Material), false);
      _isClosed = EditorGUILayout.Toggle("Is Closed", _isClosed);

      if (GUILayout.Button("Create Polygon"))
      {
        var points = GeneratePoints();
        CreateConnectedVector(points, GetPolygonName());
      }
    }

    private Vector3[] GeneratePoints()
    {
      // Calculate the points of the inscribed polygon
      var points = new Vector3[_numSides];
      var angleIncrement = 2f * Mathf.PI / _numSides;
      var angleOffset = _angle * Mathf.Deg2Rad;
      for (var i = 0; i < _numSides; i++)
      {
        var angle = angleOffset + i * angleIncrement;
        points[i] = new Vector3(_radius * Mathf.Cos(angle), _radius * Mathf.Sin(angle), 0);
      }

      return points;
    }

    public string GetPolygonName()
    {
      return $"Polygon_{_numSides}";
    }
  }
}