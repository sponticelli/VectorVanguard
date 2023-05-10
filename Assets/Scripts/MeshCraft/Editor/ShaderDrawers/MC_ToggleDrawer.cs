using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_ToggleDrawer : BaseDrawer
  {
    public override void OnGUI(
      Rect position,
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      OnGUI(editor);
      GUI.Label(position, label);
      var flag1 = prop.floatValue > 0.5;
      EditorGUI.BeginChangeCheck();
      var flag2 = GUI.Toggle(new Rect(position.xMin + EditorGUIUtility.labelWidth, position.yMin, 16f, position.height), flag1, label);
      if (!EditorGUI.EndChangeCheck())
        return;
      prop.floatValue = flag2 ? 1f : 0.0f;
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