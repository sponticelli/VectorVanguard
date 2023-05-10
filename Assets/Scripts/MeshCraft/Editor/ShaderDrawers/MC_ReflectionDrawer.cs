using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_ReflectionDrawer : BaseDrawer
  {
    private static string[] keywords = new string[4]
    {
      "MC_REFLECTION_OFF",
      "MC_REFLECTION_CUBE_SIMPLE",
      "MC_REFLECTION_CUBE_ADVANED",
      "MC_REFLECTION_UNITY_REFLECTION_PROBES"
    };
    private static string[] keywordNames = new string[4]
    {
      "Off",
      "Cubemap (Simple)",
      "Cubemap (Advanced)",
      "Unity Reflection Probe"
    };
    private static int[] intValues = new int[4]
    {
      0,
      1,
      2,
      3
    };

    public override void OnGUI(
      Rect position,
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      OnGUI(editor);
      var selectedValue = 0;
      if (targetMaterial.shaderKeywords.Contains("MC_REFLECTION_CUBE_SIMPLE"))
        selectedValue = 1;
      else if (targetMaterial.shaderKeywords.Contains("MC_REFLECTION_CUBE_ADVANED"))
        selectedValue = 2;
      else if (targetMaterial.shaderKeywords.Contains("MC_REFLECTION_UNITY_REFLECTION_PROBES"))
        selectedValue = 3;
      EditorGUI.BeginChangeCheck();
      var index = EditorGUI.IntPopup(position, " ", selectedValue, keywordNames, intValues);
      EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), "Reflection");
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(targetMaterial, "Change reflection type");
        targetMaterial.SetFloat("_MC_ReflectionEnumID", index);
        ModifyKeyWords(keywords, keywords[index]);
      }
      if (index == 0)
        return;
      var _prop1 = (MaterialProperty) null;
      var _prop2 = (MaterialProperty) null;
      var _prop3 = (MaterialProperty) null;
      var _prop4 = (MaterialProperty) null;
      var _prop5 = (MaterialProperty) null;
      var _prop6 = (MaterialProperty) null;
      LoadParameters(ref _prop1, "_Cube");
      LoadParameters(ref _prop2, "_ReflectColor");
      LoadParameters(ref _prop3, "_MC_Reflection_Strength");
      LoadParameters(ref _prop4, "_MC_Reflection_Pow");
      LoadParameters(ref _prop5, "_MC_Reflection_Fresnel_Bias");
      LoadParameters(ref _prop6, "_MC_Reflection_Roughness");
      using (new ScopedEditorGUIUtility.EditorGUIIndentLevel(1))
      {
        editor.ColorProperty(_prop2, "Color (RGB)");
        var num1 = (double) editor.RangeProperty(_prop3, "Strength");
        var num2 = (double) editor.RangeProperty(_prop5, "Fresnel Bias");
        switch (index)
        {
          case 1:
          case 2:
            editor.TextureProperty(_prop1, "Cubemap", false);
            if (index != 2)
              break;
            if (_prop1.textureValue != null && _prop1.textureValue.mipmapCount == 1)
              EditorGUILayout.HelpBox("Cubemap has no mipmaps.", MessageType.Warning);
            var num3 = (double) editor.RangeProperty(_prop6, "Roughness");
            break;
          case 3:
            var num4 = (double) editor.RangeProperty(_prop6, "Roughness");
            break;
        }
      }
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