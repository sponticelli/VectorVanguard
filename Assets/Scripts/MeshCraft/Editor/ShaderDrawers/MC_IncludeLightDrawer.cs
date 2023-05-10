using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_IncludeLightDrawer : BaseDrawer
  {
    public static string[] keywords = new string[2]
    {
      "MC_LIGHT_OFF",
      "MC_LIGHT_ON"
    };

    public override void OnGUI(
      Rect position,
      MaterialProperty prop,
      string label,
      MaterialEditor editor)
    {
      OnGUI(editor);
      var flag1 = targetMaterial.shaderKeywords.Contains("MC_LIGHT_ON");
      var flag2 = false;

 EditorGUI.BeginChangeCheck();
      using (new ScopedEditorGUIUtility.GUIEnabled(!flag2 ))
      {
        flag1 = EditorGUI.Toggle(position, " ", flag1);
        EditorGUI.LabelField(position, "Receive Light");
      }
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(targetMaterial, "Change lighting option");
        targetMaterial.SetFloat("_MC_IncludeLightEnumID", flag1 ? 1f : 0.0f);
        ModifyKeyWords(keywords, keywords[flag1 ? 1 : 0]);

       

      }

     if (flag2)
        EditorGUILayout.HelpBox("In Deferred mode Physically Based shaders wire is always rendered with Receive Light option enabled.", MessageType.Warning);
      if (((!flag1 ? 0 : (targetMaterial.shader.name.Contains("Wire Only") ? 1 : 0)) | (flag2 ? 1 : 0)) == 0 )
        return;
      var _prop1 = (MaterialProperty) null;
      var _prop2 = (MaterialProperty) null;
      LoadParameters(ref _prop1, "_Glossiness");
      LoadParameters(ref _prop2, "_Metallic");
      using (new ScopedEditorGUIUtility.EditorGUIIndentLevel(1))
      {
        editor.ShaderProperty(_prop1, "Smoothness");
        editor.ShaderProperty(_prop2, "Metallic");
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