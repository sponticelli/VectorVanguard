using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vektor.MeshFilters;


namespace Vektor.Editors
{
  [CustomEditor(typeof(VectorConnectedMeshFilter))]
  public class VectorClosedMeshFilterEditor : Editor
  {
    private List<Vector3> _worldPoints;
    private Transform _transform;
    private Vector3 _lastPosition;
    private int _numPoints;
    
    private void OnEnable()
    {
      CacheData();
    }

    private void CacheData()
    {
      _transform = ((VectorConnectedMeshFilter)target).transform;
      _worldPoints = new List<Vector3>();
      foreach (var point in ((VectorConnectedMeshFilter)target).Points)
      {
        _worldPoints.Add(_transform.TransformPoint(point));
      }
      _lastPosition = _transform.position;
      _numPoints = ((VectorConnectedMeshFilter)target).Points.Length;
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      if (GUILayout.Button("Generate"))
      {
        ((VectorConnectedMeshFilter) target).Generate(true);
      }
    }
    
    private void OnSceneGUI()
    {
      var numPoints = ((VectorConnectedMeshFilter)target).Points.Length;
      var distance = Vector3.Distance(_lastPosition, ((VectorConnectedMeshFilter)target).transform.position);
      const float epsilon = 0.001f;
      if ( distance > epsilon || numPoints != _numPoints)
      {
        CacheData();
      }
      
      
      // Draw handles for each point
      for (var i = 0; i < _worldPoints.Count; i++)
      {
        var worldPoint = _worldPoints[i];
        EditorGUI.BeginChangeCheck();
        worldPoint = Handles.PositionHandle(worldPoint, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
          // Convert the world point back to local space and update the _points list
          var localPoint = _transform.InverseTransformPoint(worldPoint);
          ((VectorConnectedMeshFilter)target).Points[i] = localPoint;
          _worldPoints[i] = worldPoint;
          ((VectorConnectedMeshFilter)target).Generate(true);
        }
        
        // Draw lines between adjacent points
        var previousColor = Handles.color;
        Handles.color = Color.blue;
        var j = (i + 1) % _worldPoints.Count;
        Handles.DrawLine(_worldPoints[i], _worldPoints[j], 1);
        Handles.color = previousColor;
      }
    }
  }
}