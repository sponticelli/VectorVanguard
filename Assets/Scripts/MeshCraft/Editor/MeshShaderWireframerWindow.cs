using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  public class MeshShaderWireframerWindow : EditorWindow
  {
    private GameObject selectedObject;

    [MenuItem("Window/LiteNinja/MeshCraft/Convert Mesh for Shader Wireframe")]
    public static void ShowWindow()
    {
      GetWindow<MeshShaderWireframerWindow>("Convert Mesh for Shader Wireframe");
    }

    private void OnGUI()
    {
      GUILayout.Label("Select a GameObject with MeshFilter and MeshRenderer (or SkinnedMeshRenderer)",
        EditorStyles.boldLabel);

      EditorGUILayout.BeginHorizontal();
      selectedObject = (GameObject)EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), true);
      EditorGUILayout.EndHorizontal();

      GUILayout.Space(10);

      if (selectedObject != null &&
          (selectedObject.GetComponent<MeshFilter>() || selectedObject.GetComponent<SkinnedMeshRenderer>()))
      {

        if (GUILayout.Button("Convert Mesh"))
        {
          var mesh =  MeshWireframer.CreateTriangleWireframeWithShader(selectedObject);
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