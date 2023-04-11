using System.IO;
using UnityEditor;
using UnityEngine;

namespace VectorVanguard.Editors
{


    public class MeshSaverWindow : EditorWindow
    {
        private GameObject selectedObject;
        private string assetName = "NewMesh";
        private string objName = "NewMesh";
        private bool saveAsAsset = true;

        [MenuItem("LiteNinja/Vektor/MeshFilter/Mesh Saver")]
        public static void ShowWindow()
        {
            GetWindow<MeshSaverWindow>("Mesh Saver");
        }

        private void OnGUI()
        {
            GUILayout.Label("Save Mesh From MeshFilter", EditorStyles.boldLabel);

            selectedObject =
                (GameObject)EditorGUILayout.ObjectField("Target GameObject", selectedObject, typeof(GameObject), true);

            if (!selectedObject) return;
            var meshFilter = selectedObject.GetComponent<MeshFilter>();

            if (meshFilter)
            {
                assetName = EditorGUILayout.TextField("Asset Name", assetName);
                objName = EditorGUILayout.TextField("OBJ Name", objName);

                saveAsAsset = EditorGUILayout.Toggle("Save as Asset", saveAsAsset);

                if (GUILayout.Button("Save Mesh"))
                {
                    var meshToSave = meshFilter.sharedMesh;

                    if (saveAsAsset)
                    {
                        SaveMeshAsAsset(meshToSave);
                    }
                    else
                    {
                        SaveMeshAsOBJ(meshToSave);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("The selected GameObject does not have a MeshFilter component.",
                    MessageType.Warning);
            }
        }

        private void SaveMeshAsAsset(Mesh mesh)
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Mesh as Asset", assetName, "asset",
                "Please enter a file name to save the mesh as an asset.");

            if (string.IsNullOrEmpty(path)) return;
            MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Mesh saved as an asset: {path}");
        }

        private void SaveMeshAsOBJ(Mesh mesh)
        {
            var path = EditorUtility.SaveFilePanel("Save Mesh as OBJ", "", objName, "obj");

            if (string.IsNullOrEmpty(path)) return;
            using var sw = new StreamWriter(path);
            sw.Write(MeshToString(mesh));
            Debug.Log($"Mesh saved as OBJ: {path}");
        }

        private string MeshToString(Mesh mesh)
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("g ").Append(mesh.name).Append("\n");

            foreach (var v in mesh.vertices)
            {
                sb.Append($"v {v.x} {v.y} {v.z}\n");
            }

            foreach (var n in mesh.normals)
            {
                sb.Append($"vn {n.x} {n.y} {n.z}\n");
            }

            foreach (var uv in mesh.uv)
            {
                sb.Append($"vt {uv.x} {uv.y}\n");
            }

            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var triangles = mesh.GetTriangles(i);

                for (var j = 0; j < triangles.Length; j += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
                }
            }

            return sb.ToString();
        }

    }
}