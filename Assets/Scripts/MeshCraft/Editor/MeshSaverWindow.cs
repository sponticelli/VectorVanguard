using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{


    public class MeshSaverWindow : EditorWindow
    {
        private GameObject selectedObject;
        private string assetName = "NewMesh";
        private string objName = "NewMesh";
        private bool saveAsAsset = true;
        private bool mergeSubMeshes = false;

        [MenuItem("Window/LiteNinja/MeshCraft/Mesh Saver")]
        public static void ShowWindow()
        {
            GetWindow<MeshSaverWindow>("Mesh Saver");
        }

        private void OnGUI()
        {
            GUILayout.Label("Save Mesh", EditorStyles.boldLabel);

            selectedObject =
                (GameObject)EditorGUILayout.ObjectField("Target GameObject", selectedObject, typeof(GameObject), true);

            mergeSubMeshes = EditorGUILayout.Toggle("Merge sub meshes", mergeSubMeshes);
            
            if (!selectedObject) return;

            var mesh = mergeSubMeshes ? MeshMerger.Merge(selectedObject)  : MeshHelper.GetMesh(selectedObject);
            
            if (mesh)
            {
                assetName = EditorGUILayout.TextField("Asset Name", assetName);
                objName = EditorGUILayout.TextField("OBJ Name", objName);

                saveAsAsset = EditorGUILayout.Toggle("Save as Asset", saveAsAsset);

                if (GUILayout.Button("Save Mesh"))
                {
                    var meshToSave = mesh;

                    if (saveAsAsset)
                    {
                        MeshEditorHelper.SaveMeshAsAsset(meshToSave, assetName);
                    }
                    else
                    {
                        MeshEditorHelper.SaveMeshAsOBJ(meshToSave, objName);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("The selected GameObject does not have a mesh.",
                    MessageType.Warning);
            }
        }

        



    }
}