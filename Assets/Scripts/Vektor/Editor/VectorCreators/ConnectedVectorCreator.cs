using UnityEditor;
using UnityEngine;
using Vektor.Meshes;
using Vektor.MeshFilters;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  public abstract class ConnectedVectorCreator : EditorWindow
  {
    protected float _lineWidth = 0.1f;
    protected Material _material;
    protected VectorMesh.JoinType _joinType;
    protected bool _isClosed = true;

    protected void CreateConnectedVector(Vector3[] points, string objectName)
    {
      // Create the GameObject and components
      var vectorObject = new GameObject(objectName);
      var vectorMeshRenderer = vectorObject.AddComponent<VectorMeshRenderer>();
      var vectorConnectedMeshFilter = vectorObject.AddComponent<VectorConnectedMeshFilter>();
      var meshFilter = vectorObject.AddComponent<MeshFilter>();
      var meshRenderer = vectorObject.AddComponent<MeshRenderer>();

      // Configure the mesh and renderer
      vectorConnectedMeshFilter.LineWidth = _lineWidth;
      vectorConnectedMeshFilter.Points = points;
      vectorConnectedMeshFilter.JoinType = _joinType;
      vectorConnectedMeshFilter.MeshFilter = meshFilter;
      vectorConnectedMeshFilter.IsClosed = _isClosed;
      vectorConnectedMeshFilter.Materials = new[] { _material };
      vectorMeshRenderer.MeshRenderer = meshRenderer;
      vectorMeshRenderer.VectorMeshFilter = vectorConnectedMeshFilter;

      vectorMeshRenderer.SetupComponents();
      vectorMeshRenderer.Generate(true);
    }
  }
}