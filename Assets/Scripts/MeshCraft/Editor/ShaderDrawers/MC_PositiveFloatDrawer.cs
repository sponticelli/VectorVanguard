using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_PositiveFloatDrawer : BaseDrawer
  {
    public override void OnGUI(
      Rect position,
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      OnGUI(editor);
      EditorGUI.BeginChangeCheck();
      var num = (double) editor.FloatProperty(prop, label);
      if (!EditorGUI.EndChangeCheck() || prop.floatValue >= 0.0)
        return;
      prop.floatValue = 0.0f;
    }

    public override float GetPropertyHeight(
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      return 0.0f;
    }
  }
}