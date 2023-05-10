using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  /// <summary>
  /// The MC_BaseDrawer class is a custom MaterialPropertyDrawer used for modifying material properties and shader keywords in the Unity editor.
  /// </summary>
  public class BaseDrawer : MaterialPropertyDrawer
  {
    /// <summary>
    /// The target material whose properties and keywords will be modified.
    /// </summary>
    protected Material targetMaterial;

    /// <summary>
    /// Sets the target material for the drawer using the provided MaterialEditor.
    /// </summary>
    /// <param name="editor">The MaterialEditor object for the target material.</param>
    protected void OnGUI(MaterialEditor editor) => targetMaterial = editor.target as Material;

    /// <summary>
    /// Modifies the shader keywords of the target material by removing the specified keywords and adding a new keyword.
    /// </summary>
    /// <param name="keywords">An array of keywords to remove from the target material's shaderKeywords.</param>
    /// <param name="newKeyword">The new keyword to add to the target material's shaderKeywords.</param>
    protected void ModifyKeyWords(IEnumerable<string> keywords, string newKeyword)
    {
      var list = targetMaterial.shaderKeywords.ToList().Except(keywords).ToList();
      list.Add(newKeyword);
      targetMaterial.shaderKeywords = list.ToArray();
    }

    protected void LoadParameters(ref MaterialProperty _prop, string _name)
    {
      var mats = new Material[1]
      {
        targetMaterial
      };
      _prop = MaterialEditor.GetMaterialProperty(mats, _name);
    }
  }
}