using UnityEditor;
using UnityEngine;
using Vektor.Meshes;
using Vektor.MeshFilters;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  public class InscribedPolygonCreator : EditorWindow
  {
    private float _radius = 1f;
    private int _numSides = 3;
    private float _lineWidth = 0.1f;
    private float _angle = 0f;
    private Material _material;
    private VectorMesh.JoinType _joinType;
    private bool _isClosed = true;

    [MenuItem("LiteNinja/Vektor/Inscribed Polygon Creator")]
    public static void ShowWindow()
    {
      GetWindow<InscribedPolygonCreator>("Inscribed Polygon Creator");
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

      if (GUILayout.Button("Create Inscribed Polygon"))
      {
        CreateInscribedPolygon();
      }
    }

    private void CreateInscribedPolygon()
    {
      if (_numSides < 3)
      {
        Debug.LogError("Number of sides must be at least 3");
        return;
      }
      // Calculate the points of the inscribed polygon
      var points = new Vector3[_numSides];
      var angleIncrement = 2f * Mathf.PI / _numSides;
      var angleOffset = _angle * Mathf.Deg2Rad;
      for (var i = 0; i < _numSides; i++)
      {
        var angle = angleOffset + i * angleIncrement;
        points[i] = new Vector3(_radius * Mathf.Cos(angle), _radius * Mathf.Sin(angle), 0);
      }

      // Create the GameObject and components
      var polygon = new GameObject(GetPolygonName());
      var vectorMeshRenderer = polygon.AddComponent<VectorMeshRenderer>();
      var vectorConnectedMeshFilter = polygon.AddComponent<VectorConnectedMeshFilter>();
      var meshFilter = polygon.AddComponent<MeshFilter>();
      var meshRenderer = polygon.AddComponent<MeshRenderer>();
      

      // Configure the mesh and renderer
      vectorConnectedMeshFilter.LineWidth = _lineWidth;
      vectorConnectedMeshFilter.Points = points;
      vectorConnectedMeshFilter.JoinType = _joinType;
      vectorConnectedMeshFilter.MeshFilter = meshFilter;
      vectorConnectedMeshFilter.IsClosed = _isClosed;
      vectorConnectedMeshFilter.Materials = new[] {_material};
      vectorMeshRenderer.MeshRenderer = meshRenderer;
      vectorMeshRenderer.VectorMeshFilter = vectorConnectedMeshFilter;
      
      vectorMeshRenderer.SetupComponents();
      vectorMeshRenderer.Generate(true);
    }
    
    public string GetPolygonName()
    {
      return $"Polygon_{_numSides}";
    }
  }
}