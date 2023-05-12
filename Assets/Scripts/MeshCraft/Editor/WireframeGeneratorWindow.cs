using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LiteNinja.MeshCraft.Wireframe;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors
{
  public class WireframeGeneratorWindow : EditorWindow
  {
    private static readonly string[] UnsupportedCharacters = {
      "~", "`", "!", "@", "#", "$", "%", "^", "&", "*", "^", "+", "=", "{", "}", "\\", "|", ";", ":", "'", "\"", ",", 
      "<", ".", ">", "?", "/"
    };

    private SourceObjectState _sourceObjectState;
    private bool _generateQuads;
    private List<MeshFilter> _meshFilters;
    private List<SkinnedMeshRenderer> _skinnedMeshRenderers;
    private int _meshCount;
    private Vector2 _scrollPos = Vector2.zero;

    [MenuItem("Window/LiteNinja/MeshCraft/Wireframe Generator", false, 311)]
    public static void ShowWindow()
    {
      var window = GetWindow<WireframeGeneratorWindow>();
      window.titleContent = new GUIContent("W Generator", "Wireframe Generator");
      window.minSize = new Vector2(340f, 320f);
      window.Show();
    }

    private void OnGUI()
    {
      EditorGUILayout.LabelField(string.Empty);
      if (GUILayout.Button("Use Selected Objects", GUILayout.MaxWidth(160f), GUILayout.MaxHeight(18f)))
      {
        GameObjectChanged();
      }

      GUILayout.Space(5f);
      switch (_sourceObjectState)
      {
        case SourceObjectState.Null:
        {
          using (new ScopedEditorGUIUtility.GUIEnabled(false))
            EditorGUILayout.LabelField("Select mesh objects inside scene and click \n'Use Selected Objects' button.",
              GUILayout.MaxHeight(36f));
          break;
        }
        case SourceObjectState.NoMeshRenderers:
        {
          using (new ScopedEditorGUIUtility.GUIEnabled(false))
            EditorGUILayout.LabelField("Selected objects have no mesh renderers attached.", GUILayout.MaxHeight(36f));
          break;
        }
        case SourceObjectState.NoMeshData:
        {
          using (new ScopedEditorGUIUtility.GUIEnabled(false))
            EditorGUILayout.LabelField("Selected objects have no mesh data.", GUILayout.MaxHeight(36f));
          break;
        }
        case SourceObjectState.OK:
        default:
          DrawSourceInfo();
          break;
      }

      ShowGenerateButton();
    }


    /// <summary>
    /// Draws information about the source meshes used in the current MeshCombiner.
    /// If there are no meshFilters or skinnedMeshRenderers assigned to the MeshCombiner,
    /// this method does nothing.
    /// </summary>
    private void DrawSourceInfo()
    {
      // If there are no meshFilters or skinnedMeshRenderers, return
      if ((_meshFilters == null || _meshFilters.Count == 0) &&
          (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Count == 0))
        return;
      
      // Define the width and height of the information box 
      GUILayout.Space(7f);
      var width = this.position.width - 6.0;
      var position = this.position;
      var height = position.height - 246.0;
      
      // Draw the information box
      GUI.Box(new Rect(2f, 105f, (float)width, (float)height), GUIContent.none);
      
      // Initialize flags and scroll position
      var meshExists = false;
      var scrollPos = this._scrollPos;
      
      // Define layout options for the scroll view
      var guiLayoutOptionArray = new GUILayoutOption[2];
      position = this.position;
      guiLayoutOptionArray[0] = GUILayout.Width(position.width - 5f);
      position = this.position;
      guiLayoutOptionArray[1] = GUILayout.Height(position.height - 250f);
      
      // Begin the scroll view 
      this._scrollPos = EditorGUILayout.BeginScrollView(scrollPos, guiLayoutOptionArray);
      
      // Set the label width and iterate over the mesh filters to display information about them 
      using (new ScopedEditorGUIUtility.EditorGUIUtilityLabelWidth(170f))
      {
        foreach (var t in _meshFilters.Where(t => t != null))
        {
          using (new ScopedEditorGUIUtility.GUIBackgroundColor(t.sharedMesh == null ? Color.red : Color.white))
            EditorGUILayout.ObjectField(t.sharedMesh == null ? " " : " " + t.sharedMesh.name, t, typeof(Mesh), 
              false);
          meshExists = true;
        }

        // Iterate over the skinned mesh renderers to display information about them 
        foreach (var t in _skinnedMeshRenderers.Where(t => t != null))
        {
          using (new ScopedEditorGUIUtility.GUIBackgroundColor(t.sharedMesh == null
                   ? Color.red
                   : Color.white))
            EditorGUILayout.ObjectField(
              t.sharedMesh == null
                ? " "
                : " " + t.sharedMesh.name, t, typeof(Mesh),
              false);
          meshExists = true;
        }
      }

      // End the scroll view 
      EditorGUILayout.EndScrollView();
      
      // If there were no meshes to display, reset the variables and dirty the editor 
      if (!meshExists)
      {
        _meshFilters = null;
        _skinnedMeshRenderers = null;
        _sourceObjectState = SourceObjectState.Null;
        EditorUtility.SetDirty(this);
      }
      // If there are meshes to display, show the mesh count and option to generate quads 
      else
      {
        using (new ScopedEditorGUIUtility.GUIEnabled(false))
          EditorGUI.LabelField(new Rect(4f, this.position.height - 138f, 140f, 40f), "Mesh Count: " + _meshCount);
        _generateQuads = EditorGUI.ToggleLeft(new Rect(4f, this.position.height - 110f, 140f, 18f), "Generate Quads",
          _generateQuads);
      }
    }

    private void ShowGenerateButton()
    {
      using (new ScopedEditorGUIUtility.GUIEnabled(_sourceObjectState == SourceObjectState.OK))
      {
        if (!GUI.Button(new Rect(6f, position.height - 50f, position.width - 12f, 40f), "Generate"))
          return;
        try
        {
          Generate();
        }
        catch (Exception ex)
        {
          Debug.LogError("Generating wireframe failed :(\n" + ex.Message);
        }

        EditorUtility.ClearProgressBar();
      }
    }

    private void Generate()
    {
      if (_sourceObjectState != SourceObjectState.OK)
        return;
      if (!Directory.Exists("Assets/_TEMP"))
        AssetDatabase.CreateFolder("Assets", "_TEMP");
      if (!Directory.Exists("Assets/_TEMP/Wireframe"))
        AssetDatabase.CreateFolder("Assets/_TEMP", "Wireframe");
      ClearConsole();
      
      GenerateFromMeshFilters();
      GenerateFromSkinnedMeshRenderers();

      EditorUtility.ClearProgressBar();
      EditorUtility.UnloadUnusedAssetsImmediate();
    }

    private void GenerateFromSkinnedMeshRenderers()
    {
      var skinnedMeshRendererDictionary = new Dictionary<SkinnedMeshRenderer, int>();
      var dictionary4 = new Dictionary<int, Mesh>();
      foreach (var t in _skinnedMeshRenderers.Where(t =>
                 !(t == null) && !(t.sharedMesh == null) && !skinnedMeshRendererDictionary.ContainsKey(t)))
      {
        skinnedMeshRendererDictionary.Add(t, t.sharedMesh.GetInstanceID());
      }

      for (var index = 0; index < _skinnedMeshRenderers.Count; ++index)
      {
        if (_skinnedMeshRenderers[index] == null || _skinnedMeshRenderers[index].sharedMesh == null) continue;

        EditorUtility.DisplayProgressBar("Hold On", _skinnedMeshRenderers[index].sharedMesh.name,
          _meshFilters.Count == 0
            ? index / (float)_skinnedMeshRenderers.Count
            : (float)(0.5 + index / (double)_skinnedMeshRenderers.Count * 0.5));
        var instanceId = _skinnedMeshRenderers[index].sharedMesh.GetInstanceID();
        if (dictionary4.ContainsKey(instanceId)) continue;

        Mesh _saveMesh;
        try
        {
          _saveMesh = _generateQuads
            ? QuadWireframeGenerator.Generate(_skinnedMeshRenderers[index].sharedMesh)
            : TriangleWireframeGenerator.Generate(_skinnedMeshRenderers[index].sharedMesh);
        }
        catch
        {
          _saveMesh = null;
        }

        if (_saveMesh == null)
        {
          Debug.LogError("Wireframe generation for '" + _skinnedMeshRenderers[index].sharedMesh.name +
                         "' has failed.\n");
        }
        else
        {
          SaveMeshAsset(_saveMesh, GetAssetSavePath(_saveMesh.name));
          dictionary4.Add(instanceId, _saveMesh);
        }
      }

      foreach (var keyValuePair in skinnedMeshRendererDictionary)
      {
        try
        {
          Undo.RecordObject(keyValuePair.Key, "Wireframe - Replacing");
          keyValuePair.Key.sharedMesh = dictionary4[keyValuePair.Value];
        }
        catch
        {
        }
      }

      dictionary4.Clear();
      skinnedMeshRendererDictionary.Clear();
    }

    private void GenerateFromMeshFilters()
    {
      var meshFilterDictionary = new Dictionary<MeshFilter, int>();
      var meshDictionary = new Dictionary<int, Mesh>();
      foreach (var t in _meshFilters.Where(t => !(t == null) && !(t.sharedMesh == null) &&
                                               !meshFilterDictionary.ContainsKey(t)))
      {
        Debug.Log(t.sharedMesh.name + " " + t.sharedMesh.GetInstanceID());
        meshFilterDictionary.Add(t, t.sharedMesh.GetInstanceID());
      }

      for (var index = 0; index < _meshFilters.Count; ++index)
      {
        if (_meshFilters[index] == null || _meshFilters[index].sharedMesh == null) continue;
        EditorUtility.DisplayProgressBar("Hold On", _meshFilters[index].sharedMesh.name,
          (float)(index / (double)_meshFilters.Count * (_skinnedMeshRenderers.Count == 0 ? 1.0 : 0.5)));
        var instanceId = _meshFilters[index].sharedMesh.GetInstanceID();
        if (meshDictionary.ContainsKey(instanceId)) continue;
        Mesh _saveMesh;
        try
        {
          Debug.Log(
            $"Generating mesh for {_meshFilters[index].sharedMesh.name} {_meshFilters[index].sharedMesh.GetInstanceID()}");
          _saveMesh = _generateQuads
            ? QuadWireframeGenerator.Generate(_meshFilters[index].sharedMesh)
            : TriangleWireframeGenerator.Generate(_meshFilters[index].sharedMesh);
        }
        catch(Exception ex)
        {
          Debug.LogException(ex);
          _saveMesh = null;
        }

        if (_saveMesh == null)
        {
          Debug.LogError("Wireframe generation for '" + _meshFilters[index].sharedMesh.name + "' has failed.\n");
        }
        else
        {
          _saveMesh.name = "WIRE_" + _saveMesh.name;
          SaveMeshAsset(_saveMesh, GetAssetSavePath(_saveMesh.name));
          meshDictionary.Add(instanceId, _saveMesh);
        }
      }

      foreach (var keyValuePair in meshFilterDictionary)
      {
        Undo.RecordObject(keyValuePair.Key, "Wireframe - Replacing");
        keyValuePair.Key.sharedMesh = meshDictionary[keyValuePair.Value];
      }
      
      meshDictionary.Clear();
      meshFilterDictionary.Clear();
    }

    private static void ClearConsole() => Type.GetType("UnityEditor.LogEntries,UnityEditor.dll")
      .GetMethod("Clear", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);

    private string GetTemporaryFolderName() => "_TEMP/Wireframe";

    private string GetTemporaryFolderPath() => "Assets/" + GetTemporaryFolderName();

    private string GetAssetSaveFolder() => GetTemporaryFolderPath();

    private string GetAssetSavePath(string _assetName)
    {
      _assetName = UnsupportedCharacters.Aggregate(_assetName, (current, t) => current.Replace(t, "_"));
      return GetAssetSaveFolder() + "/" + (string.IsNullOrEmpty(_assetName) ? "_" : _assetName) + ".asset";
    }

    private void GameObjectChanged()
    {
      _meshCount = 0;
      _meshFilters = new List<MeshFilter>();
      _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
      if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
      {
        _sourceObjectState = SourceObjectState.Null;
      }
      else
      {
        foreach (var t in Selection.gameObjects)
        {
          _meshFilters.AddRange(t.GetComponentsInChildren<MeshFilter>(true));
          _skinnedMeshRenderers.AddRange(t.GetComponentsInChildren<SkinnedMeshRenderer>(true));
        }

        if (_meshFilters.Count == 0 && _skinnedMeshRenderers.Count == 0)
        {
          _sourceObjectState = SourceObjectState.NoMeshRenderers;
        }
        else
        {
          foreach (var t in _meshFilters.Where(t => t != null && t.sharedMesh != null))
          {
            ++_meshCount;
          }

          foreach (var t in _skinnedMeshRenderers.Where(t => t != null && t.sharedMesh != null))
          {
            ++_meshCount;
          }

          _sourceObjectState = _meshCount != 0 ? SourceObjectState.OK : SourceObjectState.NoMeshData;
          _meshFilters = _meshFilters.OrderBy(mfn => mfn.name).ToList();
          _skinnedMeshRenderers = _skinnedMeshRenderers.OrderBy(skinnedMeshRenderer => skinnedMeshRenderer.name).ToList();
        }
      }
    }

    private static void SaveMeshAsset(Mesh _saveMesh, string _savePath)
    {
      _saveMesh.hideFlags = HideFlags.None;
      AssetDatabase.CreateAsset(_saveMesh, _savePath);
      EditorGUIUtility.PingObject(_saveMesh);
    }


    private enum SourceObjectState
    {
      Null,
      NoMeshRenderers,
      NoMeshData,
      OK,
    }
  }
}