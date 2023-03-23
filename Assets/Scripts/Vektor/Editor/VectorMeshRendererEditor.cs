using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vektor.MeshRenderers;

namespace Vektor.Editors
{
  [CustomEditor(typeof(VectorMeshRenderer))]
  public class VectorMeshRendererEditor : Editor
  {
    public enum ColliderType
    {
      None,
      PolygonCollider2D,
      EdgeCollider2D,
      CircleCollider2D,
      MeshCollider
    }
    
    private ColliderType _colliderType = ColliderType.None;

    
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      
      var vectorMeshRenderer = (VectorMeshRenderer)target;
      ShowSetupButton(vectorMeshRenderer);
      ShowGenerateButton(vectorMeshRenderer);
      ShowCreateColliderButton(vectorMeshRenderer);
    }

    private void ShowCreateColliderButton(VectorMeshRenderer vectorMeshRenderer)
    {
      //Draw a drop down menu to select the type of collider to create
      _colliderType = (ColliderType) EditorGUILayout.EnumPopup("Collider Type", _colliderType);
      if (GUILayout.Button("Create Collider"))
      {
        RemoveExistingColliders(vectorMeshRenderer);
        switch (_colliderType)
        {
          case ColliderType.PolygonCollider2D:
            CreatePolygonCollider2D(vectorMeshRenderer);
            break;
          case ColliderType.EdgeCollider2D:
            CreateEdgeCollider2D(vectorMeshRenderer);
            break;
          case ColliderType.CircleCollider2D:
            CreateCircleCollider2D(vectorMeshRenderer);
            break;
          case ColliderType.MeshCollider:
            CreateMeshCollider(vectorMeshRenderer);
            break;
          case ColliderType.None:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    private void CreateEdgeCollider2D(VectorMeshRenderer vectorMeshRenderer)
    {
      var edgeCollider2D = vectorMeshRenderer.gameObject.AddComponent<EdgeCollider2D>();
      var mesh = vectorMeshRenderer.VectorMeshFilter.MeshFilter.sharedMesh;
      
      var points = new Vector2[mesh.vertices.Length];
      for (var i = 0; i < mesh.vertices.Length; i++)
      {
        points[i] = mesh.vertices[i];
      }
      edgeCollider2D.points = points;
    }
    
    private void CreateCircleCollider2D(VectorMeshRenderer vectorMeshRenderer)
    {
      var circleCollider2D = vectorMeshRenderer.gameObject.AddComponent<CircleCollider2D>();
      var mesh = vectorMeshRenderer.VectorMeshFilter.MeshFilter.sharedMesh;
      
      var points = new Vector2[mesh.vertices.Length];
      //find the center of the mesh
      var center = mesh.vertices.Aggregate(Vector2.zero, (current, t) => current + (Vector2)t);
      center /= mesh.vertices.Length;
      //find the radius of the mesh
      var radius = mesh.vertices.Select(t => Vector2.Distance(center, t)).Prepend(0f).Max();

      circleCollider2D.offset = center;
      circleCollider2D.radius = radius;
      
    }

    private void CreateMeshCollider(VectorMeshRenderer vectorMeshRenderer)
    {
      var meshCollider = vectorMeshRenderer.gameObject.AddComponent<MeshCollider>();
      var mesh = vectorMeshRenderer.VectorMeshFilter.MeshFilter.sharedMesh;
      meshCollider.sharedMesh = mesh;
    }

    private void CreatePolygonCollider2D(VectorMeshRenderer vectorMeshRenderer)
    {
      var collider2D = vectorMeshRenderer.gameObject.AddComponent<PolygonCollider2D>();
      var mesh = vectorMeshRenderer.VectorMeshFilter.MeshFilter.sharedMesh;
      
      var points = new Vector2[mesh.vertices.Length];
      for (var i = 0; i < mesh.vertices.Length; i++)
      {
        points[i] = mesh.vertices[i];
      }
      collider2D.points = points;
    }
    
    private void RemoveExistingColliders(VectorMeshRenderer vectorMeshRenderer)
    {
      var existingPolygonCollider2D = vectorMeshRenderer.gameObject.GetComponent<PolygonCollider2D>();
      if (existingPolygonCollider2D != null)
      {
        DestroyImmediate(existingPolygonCollider2D);
      }
      
      var existingEdgeCollider2D = vectorMeshRenderer.gameObject.GetComponent<EdgeCollider2D>();
      if (existingEdgeCollider2D != null)
      {
        DestroyImmediate(existingEdgeCollider2D);
      }
      
      var existingCircleCollider2D = vectorMeshRenderer.gameObject.GetComponent<CircleCollider2D>();
      if (existingCircleCollider2D != null)
      {
        DestroyImmediate(existingCircleCollider2D);
      }

      var existingMeshCollider = vectorMeshRenderer.gameObject.GetComponent<MeshCollider>();
      if (existingMeshCollider != null)
      {
        DestroyImmediate(existingMeshCollider);
      }
    }

    private static void ShowGenerateButton(VectorMeshRenderer vectorMeshRenderer)
    {
      if (!GUILayout.Button("Generate")) return;
      vectorMeshRenderer.SetupComponents();
      vectorMeshRenderer.Generate(true);
    }

    private static void ShowSetupButton(VectorMeshRenderer vectorMeshRenderer)
    {
      if (vectorMeshRenderer.VectorMeshFilter != null && vectorMeshRenderer.MeshRenderer != null) return;
      if (GUILayout.Button("Setup Components"))
      {
        vectorMeshRenderer.SetupComponents();
      }
    }
  }
}