using UnityEditor;
using UnityEngine;
using Vektor.Meshes;

namespace Vektor.Editors
{
  public class SpiralVectorCreator : ConnectedVectorCreator
  {
    private float _innerRadius = 1f;
    private float _outerRadius = 2f;
    private int _numTurns = 3;
    private int _segmentsPerTurn = 10;

    [MenuItem("LiteNinja/Vektor/Spiral Creator")]
    public static void ShowWindow()
    {
      GetWindow<SpiralVectorCreator>("Spiral Creator");
    }

    private void OnGUI()
    {
      GUILayout.Label("Spiral Settings", EditorStyles.boldLabel);
      _innerRadius = EditorGUILayout.FloatField("Inner Radius", _innerRadius);
      _outerRadius = EditorGUILayout.FloatField("Outer Radius", _outerRadius);
      _numTurns = EditorGUILayout.IntField("Number of Turns", _numTurns);
      _segmentsPerTurn = EditorGUILayout.IntField("Segments Per Turn", _segmentsPerTurn);
      _lineWidth = EditorGUILayout.FloatField("Line Width", _lineWidth);
      _joinType = (VectorMesh.JoinType)EditorGUILayout.EnumPopup("Join Type", _joinType);
      _material = (Material)EditorGUILayout.ObjectField("Material", _material, typeof(Material), false);
      _isClosed = EditorGUILayout.Toggle("Is Closed", _isClosed);

      if (GUILayout.Button("Create Spiral"))
      {
        CreateSpiral();
      }
    }

    private void CreateSpiral()
    {
      if (_numTurns < 1 || _segmentsPerTurn < 1)
      {
        Debug.LogError("Number of turns and segments per turn must be at least 1");
        return;
      }

      var totalSegments = _numTurns * _segmentsPerTurn;
      var points = new Vector3[totalSegments + 1];

      var radiusStep = (_outerRadius - _innerRadius) / totalSegments;
      var angleIncrement = 2f * Mathf.PI / _segmentsPerTurn;

      for (var i = 0; i <= totalSegments; i++)
      {
        var radius = _innerRadius + radiusStep * i;
        var angle = i * angleIncrement;
        points[i] = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
      }

      _isClosed = false;
      CreateConnectedVector(points, GetSpiralName());
    }

    public string GetSpiralName()
    {
      return $"Spiral_{_numTurns}T_{_segmentsPerTurn}S";
    }
  }
}