using System;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  /// <summary>
  /// Utility class for providing scoped enabling/disabling of the GUI.
  /// </summary>
  public static class ScopedEditorGUIUtility
  {
    /// <summary>
    /// Scoped class that temporarily enables/disables the GUI.
    /// </summary>
    public class GUIEnabled : IDisposable
    {
      [SerializeField]
      private bool PreviousState { get; set; }

      public GUIEnabled(bool newState)
      {
        PreviousState = GUI.enabled;
        GUI.enabled = PreviousState && newState;
      }

      public void Dispose() => GUI.enabled = PreviousState;
    }

    /// <summary>
    /// Scoped class that temporarily sets the GUI color.
    /// </summary>
    public class GUIColor : IDisposable
    {
      [SerializeField]
      private Color PreviousColor { get; set; }

      public GUIColor(Color newColor)
      {
        PreviousColor = GUI.color;
        GUI.color = newColor;
      }

      public void Dispose() => GUI.color = PreviousColor;
    }

    /// <summary>
    /// Scoped class that temporarily sets the GUI background color.
    /// </summary>
    public class GUIBackgroundColor : IDisposable
    {
      [SerializeField]
      private Color PreviousColor { get; set; }

      public GUIBackgroundColor(Color newColor)
      {
        PreviousColor = GUI.color;
        GUI.backgroundColor = newColor;
      }

      public void Dispose() => GUI.backgroundColor = PreviousColor;
    }

    /// <summary>
    /// Scoped class that temporarily sets the font style of a GUISkin label.
    /// </summary>
    public class GUISkinLabelFontStyle : IDisposable
    {
      [SerializeField]
      private FontStyle PreviousStyle { set; get; }

      public GUISkinLabelFontStyle(FontStyle newStyle)
      {
        PreviousStyle = GUI.skin.label.fontStyle;
        GUI.skin.label.fontStyle = newStyle;
      }

      public void Dispose() => GUI.skin.label.fontStyle = PreviousStyle;
    }

    /// <summary>
    /// Scoped class that temporarily sets the normal text color of a GUISkin label.
    /// </summary>
    public class GUISkinLabelNormalTextColor : IDisposable
    {
      [SerializeField]
      private Color PreviousTextColor { set; get; }

      public GUISkinLabelNormalTextColor(Color newColor)
      {
        PreviousTextColor = GUI.skin.label.normal.textColor;
        GUI.skin.label.normal.textColor = newColor;
      }

      public void Dispose() => GUI.skin.label.normal.textColor = PreviousTextColor;
    }

    /// <summary>
    /// Scoped class that begins a horizontal group of GUILayout controls.
    /// </summary>
    public class GUILayoutBeginHorizontal : IDisposable
    {
      public GUILayoutBeginHorizontal() => GUILayout.BeginHorizontal();

      public GUILayoutBeginHorizontal(params GUILayoutOption[] layoutOptions) => GUILayout.BeginHorizontal(layoutOptions);

      public void Dispose() => GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Scoped class that begins a vertical group of GUILayout controls.
    /// </summary>
    public class GUILayoutBeginVertical : IDisposable
    {
      public GUILayoutBeginVertical() => GUILayout.BeginVertical();

      public GUILayoutBeginVertical(params GUILayoutOption[] layoutOptions) => GUILayout.BeginVertical(layoutOptions);

      public GUILayoutBeginVertical(GUIStyle style, params GUILayoutOption[] layoutOptions) => GUILayout.BeginVertical(style, layoutOptions);

      public void Dispose() => GUILayout.EndVertical();
    }

    /// <summary>
    /// Scoped class that begins a indent level for GUILayout controls.
    /// </summary>
    public class EditorGUIIndentLevel : IDisposable
    {
      [SerializeField]
      private int PreviousIndent { get; set; }

      public EditorGUIIndentLevel(int newIndent)
      {
        PreviousIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel += newIndent;
      }

      public void Dispose() => EditorGUI.indentLevel = PreviousIndent;
    }

    /// <summary>
    /// Scoped class that begins a label width for EditorGUI controls.
    /// </summary>
    public class EditorGUIUtilityLabelWidth : IDisposable
    {
      [SerializeField]
      private float PreviousWidth { get; set; }

      public EditorGUIUtilityLabelWidth(float newWidth)
      {
        PreviousWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = newWidth;
      }

      public void Dispose() => EditorGUIUtility.labelWidth = PreviousWidth;
    }

    /// <summary>
    /// A disposable class that adjusts the field width used by EditorGUIUtility while in its scope.
    /// </summary>
    public class EditorGUIUtilityFieldWidth : IDisposable
    {
      [SerializeField]
      private float PreviousWidth { get; set; }

      public EditorGUIUtilityFieldWidth(float newWidth)
      {
        PreviousWidth = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.fieldWidth = newWidth;
      }

      public void Dispose() => EditorGUIUtility.fieldWidth = PreviousWidth;
    }

    /// <summary>
    /// A disposable wrapper for EditorGUILayout's BeginHorizontal method.
    /// </summary>
    public class EditorGUILayoutBeginHorizontal : IDisposable
    {
      public EditorGUILayoutBeginHorizontal() => EditorGUILayout.BeginHorizontal();

      public EditorGUILayoutBeginHorizontal(params GUILayoutOption[] layoutOptions) => EditorGUILayout.BeginHorizontal(layoutOptions);

      public void Dispose() => EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// A class that provides IDisposable interface to begin and end a vertical layout group in the EditorGUILayout.
    /// </summary>
    public class EditorGUILayoutBeginVertical : IDisposable
    {
      public EditorGUILayoutBeginVertical() => EditorGUILayout.BeginVertical();

      public EditorGUILayoutBeginVertical(params GUILayoutOption[] layoutOptions) => EditorGUILayout.BeginVertical(layoutOptions);

      public EditorGUILayoutBeginVertical(GUIStyle style, params GUILayoutOption[] layoutOptions) => EditorGUILayout.BeginVertical(style, layoutOptions);

      public void Dispose() => EditorGUILayout.EndVertical();
    }
  }
}