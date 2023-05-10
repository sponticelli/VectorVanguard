using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class MC_LabelDrawer : BaseDrawer
  {
    private static GUIStyle guiStyle;

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
      using (new ScopedEditorGUIUtility.GUIBackgroundColor(EditorGUIUtility.isProSkin ? Color.grey * 0.9f : Color.grey))
      {
        guiStyle ??= new GUIStyle(EditorStyles.label)
        {
          alignment = TextAnchor.MiddleLeft,
          fontSize = 13,
          normal =
          {
            textColor = Color.white * 0.9f
          },
          fontStyle = FontStyle.Bold
        };
        
        //Add some padding
        position.yMin += 2f;
        position.yMax -= 2f;
        //Draw a separator
        var separatorRect = new Rect(position.xMin, position.yMin, position.width, 1f);
        EditorGUI.DrawRect(separatorRect, Color.white * 0.5f);
        //Add some padding
        position.yMin += 2f;
        position.yMax -= 2f;
        position.height += 2f;
        GUI.Label(position, label, guiStyle);
        
        
      }
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
      return 18f;
    }
  }
}