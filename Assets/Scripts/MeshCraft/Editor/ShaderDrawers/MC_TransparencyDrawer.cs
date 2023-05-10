using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_TransparencyDrawer : BaseDrawer
  {
    private static string[] keywords =
    {
      "MC_TRANSPARENCY_OFF",
      "MC_TRANSPARENCY_ON"
    };

    private static string[] keywordNames =
    {
      "Off",
      "On"
    };

    private static int[] intValues = { 0, 1 };

    private static string[] uvSetNames =
    {
      "uv0",
      "uv1"
    };

    private static int[] uvSetValues = { 0, 1 };

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
      OnGUI(editor);
      var selectedValue = targetMaterial.shaderKeywords.Contains("MC_TRANSPARENCY_ON") ? 1 : 0;
      EditorGUI.BeginChangeCheck();
      var index = EditorGUI.IntPopup(position, " ", selectedValue, keywordNames, intValues);
      EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), "Use Texture Alpha");
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(targetMaterial, "Change shader");
        targetMaterial.SetFloat("_MC_TransparencyEnumID", index);
        ModifyKeyWords(keywords, keywords[index]);
      }

      if (index == 0)
        return;
      var _prop = (MaterialProperty)null;
      LoadParameters(ref _prop, "_MC_TransparentTex_Alpha_Offset");
      using (new ScopedEditorGUIUtility.EditorGUIIndentLevel(1))
      {
        var flag1 = targetMaterial.GetInt("_MC_TransparentTex_Invert") != 0;
        EditorGUI.BeginChangeCheck();
        var flag2 = EditorGUILayout.Toggle("Invert", flag1);
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject(targetMaterial, "Change shader");
          targetMaterial.SetFloat("_MC_TransparentTex_Invert", flag2 ? 1f : 0.0f);
        }

        var num = (double)editor.RangeProperty(_prop, "Value Offset");
      }
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
      return 16f;
    }
  }
}