using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_VertexLightAndAmbientDrawer : BaseDrawer
  {
    private static string[] keywords = new string[2]
    {
      "MC_VERTEX_LIGHT_AND_AMBIENT_OFF",
      "MC_VERTEX_LIGHT_AND_AMBIENT_ON"
    };

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
      OnGUI(editor);
      var flag1 = targetMaterial.shaderKeywords.Contains("MC_VERTEX_LIGHT_AND_AMBIENT_ON");
      EditorGUI.BeginChangeCheck();
      var flag2 = EditorGUI.Toggle(position, " ", flag1);
      EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), "Receive Ambient Lighting");
      if (!EditorGUI.EndChangeCheck())
        return;
      targetMaterial.SetFloat("_MC_VertexLightAndAmbientID", flag2 ? 1f : 0.0f);
      ModifyKeyWords(keywords, keywords[flag2 ? 1 : 0]);
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
      return 16f;
    }
  }
}