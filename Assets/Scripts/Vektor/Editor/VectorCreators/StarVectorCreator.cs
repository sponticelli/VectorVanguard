using UnityEditor;
using UnityEngine;
using Vektor.Meshes;

namespace Vektor.Editors
{
  public class StarVectorCreator : ConnectedVectorCreator
  {
    private float _outerRadius = 1f;
    private float _innerRadius = 0.5f;
    private int _numPoints = 5;
    private float _angle = 0f;

    [MenuItem("LiteNinja/Vektor/Star Creator")]
    public static void ShowWindow()
    {
      GetWindow<StarVectorCreator>("Star Creator");
    }

    private void OnGUI()
    {
      GUILayout.Label("Star Settings", EditorStyles.boldLabel);
      _outerRadius = EditorGUILayout.FloatField("Outer Radius", _outerRadius);
      _innerRadius = EditorGUILayout.FloatField("Inner Radius", _innerRadius);
      _numPoints = EditorGUILayout.IntField("Number of Points", _numPoints);
      _angle = EditorGUILayout.IntSlider("Angle", (int)_angle, -90, 90);
      _lineWidth = EditorGUILayout.FloatField("Line Width", _lineWidth);
      _joinType = (VectorMesh.JoinType)EditorGUILayout.EnumPopup("Join Type", _joinType);
      _material = (Material)EditorGUILayout.ObjectField("Material", _material, typeof(Material), false);
      _isClosed = EditorGUILayout.Toggle("Is Closed", _isClosed);

      if (GUILayout.Button("Create Star"))
      {
        CreateStar();
      }
    }

    private void CreateStar()
    {
      if (_numPoints < 2)
      {
        Debug.LogError("Number of points must be at least 2");
        return;
      }
      // Calculate the points of the star
      var points = new Vector3[_numPoints * 2];
      var angleIncrement = Mathf.PI / _numPoints;
      var angleOffset = _angle * Mathf.Deg2Rad;
      for (var i = 0; i < _numPoints * 2; i++)
      {
        var angle = angleOffset + i * angleIncrement;
        var radius = (i % 2 == 0) ? _outerRadius : _innerRadius;
        points[i] = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
      }

      // Call the CreateConnectedVector method with the points and object name
      CreateConnectedVector(points, GetStarName());
    }

    public string GetStarName()
    {
      return $"Star_{_numPoints}";
    }
  }
}