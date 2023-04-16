using UnityEditor;
using UnityEngine;
using VectorVanguard.VFX;

namespace VectorVanguard.Attributes
{
  [CustomEditor(typeof(NoiseMaterialController))]
  public class NoiseMaterialControllerEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      if (GUILayout.Button("Generate"))
      {
        var controller = (NoiseMaterialController)target;
        
        if (controller.MeshRenderer == null)
        {
          controller.MeshRenderer = controller.GetComponent<MeshRenderer>();
        }
        
        if (controller.MeshRenderer == null)
        {
          Debug.LogError("MeshRenderer is null. Assign it manually or add the component to the GameObject.");
          return;
        }

        if (controller.MeshRenderer.sharedMaterial == null || 
            controller.MeshRenderer.sharedMaterial.shader != controller.Shader)
        {
          controller.CreateMaterial();
        }

        controller.SetShaderData();
      }
    }
  }
}