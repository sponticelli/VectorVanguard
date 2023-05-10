using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_ImprovedBlendDrawer : BaseDrawer
  {
    public override void OnGUI(
      Rect position,
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      OnGUI(editor);
      var flag1 = targetMaterial.shader.name.Contains("Improved");
      EditorGUI.BeginChangeCheck();
      var flag2 = EditorGUI.Toggle(position, " ", flag1);
      EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), "Improved Rendering (2 Pass)");
      if (!EditorGUI.EndChangeCheck())
        return;
      var shader = !flag2 ? Shader.Find(targetMaterial.shader.name.Replace(" Improved", string.Empty)) : Shader.Find(targetMaterial.shader.name + " Improved");
      if (!(shader != null))
        return;
      Undo.RecordObject(targetMaterial, "Change shader");
      targetMaterial.shader = shader;
      EditorUtility.SetDirty(editor);
    }

    public override float GetPropertyHeight(
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      return 16f;
    }
  }
}