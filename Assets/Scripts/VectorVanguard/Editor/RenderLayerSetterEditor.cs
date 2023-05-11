using System.Linq;
using UnityEditor;
using UnityEngine;
using VectorVanguard.Utils;

namespace VectorVanguard.Editors
{
  [CustomEditor(typeof(RenderLayerSetter))]
  public class RenderLayerSetterEditor : Editor
  {
    private SerializedProperty _sortingOrder;
    private SerializedProperty _sortingLayerID;
    private RenderLayerSetter _renderLayerSetter;

    private void OnEnable()
    {
      _sortingOrder = serializedObject.FindProperty("_sortingOrder");
      _sortingLayerID = serializedObject.FindProperty("_sortingLayerID");
      _renderLayerSetter = (RenderLayerSetter) target;
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUI.BeginChangeCheck();
      var newSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", GetSortingLayerIndex(_renderLayerSetter.SortingLayer.id), GetSortingLayerNames());
      if (EditorGUI.EndChangeCheck())
      {
        _sortingLayerID.intValue = SortingLayer.layers[newSortingLayerIndex].id;
      }

      EditorGUI.BeginChangeCheck();
      EditorGUILayout.PropertyField(_sortingOrder, new GUIContent("Sorting Order"));
      if (EditorGUI.EndChangeCheck())
      {
        _sortingOrder.intValue = Mathf.Clamp(_sortingOrder.intValue, -32768, 32767);
      }
      
      serializedObject.ApplyModifiedProperties();

      if (GUI.changed)
      {
        _renderLayerSetter.SetSortingInfo();
      }
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