using System;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class CommonDrawerData
  {
    public Rect position;
    public MaterialProperty prop;
    public string label;
    public MaterialEditor editor;

    public void OnGUI(Action<CommonDrawerData> getValueAction, Action<CommonDrawerData> setValueAction)
    {
      if (!prop.hasMixedValue)
      {
        getValueAction(this);
        setValueAction(this);
        return;
      }

      EditorGUI.BeginChangeCheck();
      EditorGUI.showMixedValue = prop.hasMixedValue;
      getValueAction(this);
      EditorGUI.showMixedValue = false;
      if (!EditorGUI.EndChangeCheck()) return;
      setValueAction(this);
    }
  }
}