using System;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  public class MeshWireframerWindow : EditorWindow
  {
    public enum WireframeType
    {
      Triangle,
      Quad,
    }
    
    private GameObject selectedObject;

    private WireframeType wireframeType = WireframeType.Triangle;

    [MenuItem("Window/LiteNinja/MeshCraft/Convert Mesh to Wireframe Geometry")]
    public static void ShowWindow()
    {
      GetWindow<MeshWireframerWindow>("Convert Mesh to Wireframe Geometry");
    }

    private void OnGUI()
    {
      GUILayout.Label("Select a GameObject with MeshFilter and MeshRenderer (or SkinnedMeshRenderer)",
        EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();
      selectedObject = (GameObject)EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), true);
      //Add a select box to determine the wireframe type
      wireframeType = (WireframeType)EditorGUILayout.EnumPopup(wireframeType);
      
      EditorGUILayout.EndHorizontal();

      GUILayout.Space(10);

      if (selectedObject != null &&
          (selectedObject.GetComponent<MeshFilter>() || selectedObject.GetComponent<SkinnedMeshRenderer>()))
      {
        if (GUILayout.Button("Convert Mesh"))
        {
          Mesh mesh;
          switch (wireframeType)
          {
            case WireframeType.Triangle:
              mesh = MeshWireframer.CreateTriangleWireframeMesh(selectedObject);
              break;
            case WireframeType.Quad:
              mesh = MeshWireframer.CreateQuadWireframeMesh(selectedObject);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
          
          MeshEditorHelper.SaveMeshAsAsset(mesh, "Wireframe");
        }
      }
      else
      {
        GUILayout.Label("Select a GameObject with MeshFilter and MeshRenderer (or SkinnedMeshRenderer)",
          EditorStyles.boldLabel);
      }
    }
  }
}