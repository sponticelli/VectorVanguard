using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  public class FlatConvexHullWindow : EditorWindow
  {
    private GameObject selectedObject;
    private MeshSilhouette.Axis axis = MeshSilhouette.Axis.NegativeY;

    [MenuItem("Window/LiteNinja/MeshCraft/Convert Mesh for Flat Convex Hull")]
    public static void ShowWindow()
    {
      GetWindow<FlatConvexHullWindow>("Convert Mesh for Flat Convex Hull");
    }

    private void OnGUI()
    {
      GUILayout.Label("Select a GameObject with MeshFilter and MeshRenderer (or SkinnedMeshRenderer)",
        EditorStyles.boldLabel);

      EditorGUILayout.BeginVertical();
      selectedObject = (GameObject)EditorGUILayout.ObjectField(selectedObject, typeof(GameObject), true);
      axis = (MeshSilhouette.Axis)EditorGUILayout.EnumPopup(axis);
      EditorGUILayout.EndVertical();

      GUILayout.Space(10);

      if (selectedObject != null &&
          (selectedObject.GetComponent<MeshFilter>() || selectedObject.GetComponent<SkinnedMeshRenderer>()))
      {

        if (GUILayout.Button("Convert Mesh"))
        {
          var name = selectedObject.name + "_FlatConvexHull_" + axis.ToString();
          
          var mesh =  MeshSilhouette.CreateSilhouetteMesh(selectedObject, axis);
          MeshEditorHelper.SaveMeshAsAsset(mesh, name);
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