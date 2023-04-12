using System.Linq;
using UnityEditor;
using UnityEngine;
using VectorVanguard.Utils;

namespace VectorVanguard.Editors
{
  [CustomEditor(typeof(RenderLayerSetter))]
  public class RenderLayerSetterEditor : Editor
  {
    SerializedProperty sortingOrder;
    SerializedProperty sortingLayerID;

    private void OnEnable()
    {
      sortingOrder = serializedObject.FindProperty("_sortingOrder");
      sortingLayerID = serializedObject.FindProperty("_sortingLayerID");
    }

    public override void OnInspectorGUI()
    {
      // Draw the default inspector
      //DrawDefaultInspector();

      // Draw the sorting layer dropdown menu
      EditorGUI.BeginChangeCheck();
      EditorGUI.BeginProperty(new Rect(), GUIContent.none, sortingLayerID);
      var newSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", GetSortingLayerIndex(sortingLayerID.intValue), GetSortingLayerNames());
      EditorGUI.EndProperty();
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(target, "Changed Sorting Layer");
        sortingLayerID.intValue = SortingLayer.layers[newSortingLayerIndex].id;
        serializedObject.ApplyModifiedProperties();
      }

      // Draw the sorting order field
      EditorGUILayout.PropertyField(sortingOrder);
    }

    private string[] GetSortingLayerNames()
    {
      return SortingLayer.layers.Select(layer => layer.name).ToArray();
    }

    private int GetSortingLayerIndex(int sortingLayerID)
    {
      for (var i = 0; i < SortingLayer.layers.Length; i++)
      {
        if (SortingLayer.layers[i].id == sortingLayerID)
        {
          return i;
        }
      }
      return 0;
    }
  }
}