using UnityEditor;
using UnityEngine;
using VectorVanguard.VFX;

namespace VectorVanguard.Attributes
{
  [CustomEditor(typeof(NoiseMaterialController))]
  public class NoiseMaterialControllerEditor : Editor
  {
    private NoiseMaterialController _controller;

    private void OnEnable()
    {
      _controller = target as NoiseMaterialController;
    }
    
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      GUILayout.Space(10);
      if (GUILayout.Button("Generate"))
      {
        Generate();
      }
      
      if (GUILayout.Button("Save"))
      {
        Save();
      }
    }

    private void Save()
    {
      var material = _controller.MeshRenderer.sharedMaterial;

      if (material != null)
      {
        var path = EditorUtility.SaveFilePanelInProject("Save Material", "New Material", "mat", "Save the material as a new asset");
        if (!string.IsNullOrEmpty(path))
        {
          AssetDatabase.CreateAsset(material, path);
          AssetDatabase.SaveAssets();
          Debug.Log($"Material saved to {path}");
        }
      }
      else
      {
        Debug.LogWarning("There is no material to save.");
      }
    }

    private void Generate()
    {
      _controller = (NoiseMaterialController)target;

      if (_controller.MeshRenderer == null)
      {
        _controller.MeshRenderer = _controller.GetComponent<MeshRenderer>();
      }

      if (_controller.MeshRenderer == null)
      {
        Debug.LogError("MeshRenderer is null. Assign it manually or add the component to the GameObject.");
        return;
      }

      if (_controller.MeshRenderer.sharedMaterial == null ||
          _controller.MeshRenderer.sharedMaterial.shader != _controller.Shader)
      {
        _controller.CreateMaterial();
      }

      _controller.SetShaderData();
    }
  }
}