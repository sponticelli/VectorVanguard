using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  internal class MC_UVScrollDrawer : BaseDrawer
  {
    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
      position.y -= 56f;
      position.width -= 66f;
      var vectorValue = (Vector2) prop.vectorValue;
      const float width = 80f;
      var x = position.x + width;
      var totalPosition = new Rect(position.x + EditorGUI.indentLevel * 15f, position.y, width, 16f);
      var position1 = new Rect(x, position.y, position.width - width, 16f);
      var label1 = new GUIContent(label + "Scroll");
      EditorGUI.PrefixLabel(totalPosition, label1);
      EditorGUI.BeginChangeCheck();
      var vector2 = EditorGUI.Vector2Field(position1, GUIContent.none, vectorValue);
      if (!EditorGUI.EndChangeCheck())
        return;
      prop.vectorValue = vector2;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
      return -8f;
    }
  }
}