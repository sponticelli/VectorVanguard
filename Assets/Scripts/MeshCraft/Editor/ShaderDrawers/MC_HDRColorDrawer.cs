using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_HDRColorDrawer : BaseDrawer
  {
    private const string FieldNameRGBA = "HDR Color (RGBA)";
    private const string FieldNameRGB = "HDR Color (RGB)";
    private Color _curVal;

    protected void GetCurrentValue(CommonDrawerData data)
    {
      var target = data.editor.target;
      _curVal = EditorGUILayout.ColorField(new GUIContent(data.label), data.prop.colorValue, true, true, true);
    }

    protected void SetCurrentValue(CommonDrawerData data) => data.prop.colorValue = _curVal;

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
      OnGUI(editor);
      new CommonDrawerData
      {
        position = position,
        prop = prop,
        label = targetMaterial.shader.name.Contains("Cutout") ? FieldNameRGB : FieldNameRGBA,
        editor = editor
      }.OnGUI(GetCurrentValue, SetCurrentValue);
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
      return 0.0f;
    }
  }
}