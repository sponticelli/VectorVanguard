using System.IO;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  public static class MeshEditorHelper
  {
    public static void SaveMeshAsAsset(Mesh mesh, string assetName)
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

    public static void SaveMeshAsOBJ(Mesh mesh, string objName)
    {
      var path = EditorUtility.SaveFilePanel("Save Mesh as OBJ", "", objName, "obj");

      if (string.IsNullOrEmpty(path)) return;
      using var sw = new StreamWriter(path);
      sw.Write(MeshHelper.MeshToString(mesh));
      Debug.Log($"Mesh saved as OBJ: {path}");
    }
  }
}