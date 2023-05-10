using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LiteNinja.MeshCraft.Editors.ShaderDrawers
{
  public class StandardShaderGUI : ShaderGUI
  {
    private MaterialProperty blendMode;
    private MaterialProperty albedoMap;
    private MaterialProperty albedoColor;
    private MaterialProperty alphaCutoff;
    private MaterialProperty specularMap;
    private MaterialProperty specularColor;
    private MaterialProperty metallicMap;
    private MaterialProperty metallic;
    private MaterialProperty smoothness;
    private MaterialProperty smoothnessScale;
    private MaterialProperty smoothnessMapChannel;
    private MaterialProperty highlights;
    private MaterialProperty reflections;
    private MaterialProperty bumpScale;
    private MaterialProperty bumpMap;
    private MaterialProperty occlusionStrength;
    private MaterialProperty occlusionMap;
    private MaterialProperty heigtMapScale;
    private MaterialProperty heightMap;
    private MaterialProperty emissionColorForRendering;
    private MaterialProperty emissionMap;
    private MaterialProperty detailMask;
    private MaterialProperty detailAlbedoMap;
    private MaterialProperty detailNormalMapScale;
    private MaterialProperty detailNormalMap;
    private MaterialProperty uvSetSecondary;
    
    private MaterialProperty _MC_LineWidth;
    private MaterialProperty _MC_Color;
    private MaterialProperty _MC_WireTex;
    private MaterialProperty _MC_WireTex_UVSet;
    private MaterialProperty _MC_EmissionStrength;
    private MaterialProperty _MC_TransparencyEnumID;
    private MaterialProperty _MC_FresnelEnumID;
    private MaterialProperty _MC_DistanceFade;
    private MaterialProperty _MC_DynamicGIEnumID;
    private MaterialEditor m_MaterialEditor;
    private WorkflowMode m_WorkflowMode;
    private bool m_FirstTimeApply = true;
    private static GUIStyle guiStyle;

    public void FindProperties(MaterialProperty[] props)
    {
      blendMode = FindProperty("_Mode", props);
      albedoMap = FindProperty("_MainTex", props);
      albedoColor = FindProperty("_Color", props);
      alphaCutoff = FindProperty("_Cutoff", props);
      specularMap = FindProperty("_SpecGlossMap", props, false);
      specularColor = FindProperty("_SpecColor", props, false);
      metallicMap = FindProperty("_MetallicGlossMap", props, false);
      metallic = FindProperty("_Metallic", props, false);
      m_WorkflowMode = specularMap == null || specularColor == null
        ? (metallicMap == null || metallic == null ? WorkflowMode.Dielectric : WorkflowMode.Metallic)
        : WorkflowMode.Specular;
      smoothness = FindProperty("_Glossiness", props);
      smoothnessScale = FindProperty("_GlossMapScale", props, false);
      smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
      highlights = FindProperty("_SpecularHighlights", props, false);
      reflections = FindProperty("_GlossyReflections", props, false);
      bumpScale = FindProperty("_BumpScale", props);
      bumpMap = FindProperty("_BumpMap", props);
      heigtMapScale = FindProperty("_Parallax", props);
      heightMap = FindProperty("_ParallaxMap", props);
      occlusionStrength = FindProperty("_OcclusionStrength", props);
      occlusionMap = FindProperty("_OcclusionMap", props);
      emissionColorForRendering = FindProperty("_EmissionColor", props);
      emissionMap = FindProperty("_EmissionMap", props);
      detailMask = FindProperty("_DetailMask", props);
      detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
      detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
      detailNormalMap = FindProperty("_DetailNormalMap", props);
      uvSetSecondary = FindProperty("_UVSec", props);
      _MC_LineWidth = FindProperty("_MC_LineWidth", props);
      _MC_Color = FindProperty("_MC_Color", props);
      _MC_WireTex = FindProperty("_MC_WireTex", props);
      _MC_WireTex_UVSet = FindProperty("_MC_WireTex_UVSet", props);
      _MC_EmissionStrength = FindProperty("_MC_EmissionStrength", props);
      _MC_TransparencyEnumID = FindProperty("_MC_TransparencyEnumID", props);
      _MC_FresnelEnumID = FindProperty("_MC_FresnelEnumID", props);
      _MC_DistanceFade = FindProperty("_MC_DistanceFade", props);
      _MC_DynamicGIEnumID = FindProperty("_MC_DynamicGIEnumID", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
      FindProperties(props);
      m_MaterialEditor = materialEditor;
      var target = materialEditor.target as Material;
      if (m_FirstTimeApply)
      {
        MaterialChanged(target, m_WorkflowMode);
        m_FirstTimeApply = false;
      }

      ShaderPropertiesGUI(target);
    }

    public void ShaderPropertiesGUI(Material material)
    {
      EditorGUIUtility.labelWidth = 0.0f;
      EditorGUI.BeginChangeCheck();
      BlendModePopup();
      GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
      DoAlbedoArea(material);
      DoSpecularMetallicArea();
      m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap,
        bumpMap.textureValue != null ? bumpScale : null);
      m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap,
        occlusionMap.textureValue != null
          ? occlusionStrength
          : null);
      DoEmissionArea(material);
      EditorGUI.BeginChangeCheck();
      m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
      if (EditorGUI.EndChangeCheck())
        emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset;
      EditorGUILayout.Space();
      GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
      m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
      m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
      m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
      m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);
      GUILayout.Label(Styles.forwardText, EditorStyles.boldLabel);
      if (highlights != null)
        m_MaterialEditor.ShaderProperty(highlights, Styles.highlightsText);
      if (reflections != null)
        m_MaterialEditor.ShaderProperty(reflections, Styles.reflectionsText);
      if (EditorGUI.EndChangeCheck())
      {
        foreach (Material target in blendMode.targets)
          MaterialChanged(target, m_WorkflowMode);
      }

      EditorGUILayout.Space();
      GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
      m_MaterialEditor.RenderQueueField();
      m_MaterialEditor.EnableInstancingField();
      DrawWireframeGUI(m_MaterialEditor.target as Material, m_MaterialEditor);
    }

    private void DetermineWorkflow(MaterialProperty[] props)
    {
      if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
        m_WorkflowMode = WorkflowMode.Specular;
      else if (FindProperty("_MetallicGlossMap", props, false) != null &&
               FindProperty("_Metallic", props, false) != null)
        m_WorkflowMode = WorkflowMode.Metallic;
      else
        m_WorkflowMode = WorkflowMode.Dielectric;
    }

    public override void AssignNewShaderToMaterial(
      Material material,
      Shader oldShader,
      Shader newShader)
    {
      if (material.HasProperty("_Emission"))
        material.SetColor("_EmissionColor", material.GetColor("_Emission"));
      base.AssignNewShaderToMaterial(material, oldShader, newShader);
      if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
      {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
      }
      else
      {
        var blendMode = BlendMode.Opaque;
        if (oldShader.name.Contains("/Transparent/Cutout/"))
          blendMode = BlendMode.Cutout;
        else if (oldShader.name.Contains("/Transparent/"))
          blendMode = BlendMode.Fade;
        material.SetFloat("_Mode", (float)blendMode);
        DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Material[1]
        {
          material
        }));
        MaterialChanged(material, m_WorkflowMode);
      }
    }

    private void BlendModePopup()
    {
      EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
      var floatValue = (BlendMode)this.blendMode.floatValue;
      EditorGUI.BeginChangeCheck();
      var blendMode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)floatValue, Styles.blendNames);
      if (EditorGUI.EndChangeCheck())
      {
        m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
        this.blendMode.floatValue = (float)blendMode;
      }

      EditorGUI.showMixedValue = false;
    }

    private void DoAlbedoArea(Material material)
    {
      m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
      if ((int)material.GetFloat("_Mode") != 1)
        return;
      m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, 3);
    }

    private void DoEmissionArea(Material material)
    {
      if (!m_MaterialEditor.EmissionEnabledProperty())
        return;
      var flag = emissionMap.textureValue != null;
      m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, false);
      var maxColorComponent = emissionColorForRendering.colorValue.maxColorComponent;
      if (emissionMap.textureValue != null && !flag &&
          maxColorComponent <= 0.0)
        emissionColorForRendering.colorValue = Color.white;
      m_MaterialEditor.LightmapEmissionFlagsProperty(2, true);
    }

    private void DoSpecularMetallicArea()
    {
      var flag1 = false;
      switch (m_WorkflowMode)
      {
        case WorkflowMode.Specular:
          flag1 = specularMap.textureValue != null;
          m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap,
            flag1 ? null : specularColor);
          break;
        case WorkflowMode.Metallic:
          flag1 = metallicMap.textureValue != null;
          m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap,
            flag1 ? null : metallic);
          break;
      }

      var flag2 = flag1 || smoothnessMapChannel != null && (int)smoothnessMapChannel.floatValue == 1;
      const int labelIndent1 = 2;
      m_MaterialEditor.ShaderProperty(flag2 ? smoothnessScale : smoothness,
        flag2 ? Styles.smoothnessScaleText : Styles.smoothnessText, labelIndent1);
      const int labelIndent2 = labelIndent1 + 1;
      if (smoothnessMapChannel == null)
        return;
      m_MaterialEditor.ShaderProperty(smoothnessMapChannel, Styles.smoothnessMapChannelText, labelIndent2);
    }

    public static void SetupMaterialWithBlendMode(
      Material material,
      BlendMode blendMode)
    {
      switch (blendMode)
      {
        case BlendMode.Opaque:
          material.SetOverrideTag("RenderType", "");
          material.SetInt("_SrcBlend", 1);
          material.SetInt("_DstBlend", 0);
          material.SetInt("_ZWrite", 1);
          material.DisableKeyword("_ALPHATEST_ON");
          material.DisableKeyword("_ALPHABLEND_ON");
          material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = -1;
          break;
        case BlendMode.Cutout:
          material.SetOverrideTag("RenderType", "TransparentCutout");
          material.SetInt("_SrcBlend", 1);
          material.SetInt("_DstBlend", 0);
          material.SetInt("_ZWrite", 1);
          material.EnableKeyword("_ALPHATEST_ON");
          material.DisableKeyword("_ALPHABLEND_ON");
          material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = 2450;
          break;
        case BlendMode.Fade:
          material.SetOverrideTag("RenderType", "Transparent");
          material.SetInt("_SrcBlend", 5);
          material.SetInt("_DstBlend", 10);
          material.SetInt("_ZWrite", 0);
          material.DisableKeyword("_ALPHATEST_ON");
          material.EnableKeyword("_ALPHABLEND_ON");
          material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = 3000;
          break;
        case BlendMode.Transparent:
          material.SetOverrideTag("RenderType", "Transparent");
          material.SetInt("_SrcBlend", 1);
          material.SetInt("_DstBlend", 10);
          material.SetInt("_ZWrite", 0);
          material.DisableKeyword("_ALPHATEST_ON");
          material.DisableKeyword("_ALPHABLEND_ON");
          material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
          material.renderQueue = 3000;
          break;
      }
    }

    private static SmoothnessMapChannel GetSmoothnessMapChannel(
      Material material)
    {
      return (int)material.GetFloat("_SmoothnessTextureChannel") == 1
        ? SmoothnessMapChannel.AlbedoAlpha
        : SmoothnessMapChannel.SpecularMetallicAlpha;
    }

    private static void SetMaterialKeywords(
      Material material,
      WorkflowMode workflowMode)
    {
      SetKeyword(material, "_NORMALMAP",
        (bool)(Object)material.GetTexture("_BumpMap") ||
        (bool)(Object)material.GetTexture("_DetailNormalMap"));
      switch (workflowMode)
      {
        case WorkflowMode.Specular:
          SetKeyword(material, "_SPECGLOSSMAP", (bool)(Object)material.GetTexture("_SpecGlossMap"));
          break;
        case WorkflowMode.Metallic:
          SetKeyword(material, "_METALLICGLOSSMAP", (bool)(Object)material.GetTexture("_MetallicGlossMap"));
          break;
      }

      SetKeyword(material, "_PARALLAXMAP", (bool)(Object)material.GetTexture("_ParallaxMap"));
      SetKeyword(material, "_DETAIL_MULX2",
        (bool)(Object)material.GetTexture("_DetailAlbedoMap") ||
        (bool)(Object)material.GetTexture("_DetailNormalMap"));
      MaterialEditor.FixupEmissiveFlag(material);
      var state = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) ==
                  MaterialGlobalIlluminationFlags.None;
      SetKeyword(material, "_EMISSION", state);
      if (!material.HasProperty("_SmoothnessTextureChannel"))
        return;
      SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
        GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
    }

    private static void MaterialChanged(
      Material material,
      WorkflowMode workflowMode)
    {
      SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
      SetMaterialKeywords(material, workflowMode);
    }

    private static void SetKeyword(Material m, string keyword, bool state)
    {
      if (state)
        m.EnableKeyword(keyword);
      else
        m.DisableKeyword(keyword);
    }

    private void DrawWireframeGUI(Material material, MaterialEditor editor)
    {
      DrawWireOptions_Size(material, editor);
      DrawWireOptions_Color(material, editor);
      DrawWireOptions_Transparency(material, editor);
    }

    private void DrawWireOptions_Size(Material targetMaterial, MaterialEditor editor)
    {
      GUILayout.Label("Wire Source Options", EditorStyles.boldLabel);
      EditorGUI.BeginChangeCheck();
      var num = (double)editor.FloatProperty(_MC_LineWidth, "Size");
      if (EditorGUI.EndChangeCheck() && _MC_LineWidth.floatValue < 0.0)
        _MC_LineWidth.floatValue = 0.0f;
    }

    private void DrawWireOptions_Color(Material targetMaterial, MaterialEditor editor)
    {
      GUILayout.Space(5f);
      GUILayout.Label("Wire Visual Options", EditorStyles.boldLabel);
      editor.TexturePropertySingleLine(new GUIContent("Tex/Color/Emission", "Texture, Color, Emission"),
        _MC_WireTex, _MC_Color, _MC_EmissionStrength);
      if (_MC_EmissionStrength.floatValue < 0.0)
        _MC_EmissionStrength.floatValue = 0.0f;
      using (new ScopedEditorGUIUtility.GUIEnabled(_MC_WireTex.textureValue !=
                                                   null))
      {
        editor.TextureScaleOffsetProperty(_MC_WireTex);
        editor.ShaderProperty(_MC_WireTex_UVSet, Styles.uvSetLabel.text);
      }

      editor.ShaderProperty(_MC_DynamicGIEnumID, "Global Illumination");
    }

    private void DrawWireOptions_Transparency(Material targetMaterial, MaterialEditor editor)
    {
      GUILayout.Space(5f);
      GUILayout.Label("Wire Transparency Options", EditorStyles.boldLabel);
      editor.ShaderProperty(_MC_TransparencyEnumID, "Use Texture Alpha");
      editor.ShaderProperty(_MC_FresnelEnumID, "Fresnel");
      editor.ShaderProperty(_MC_DistanceFade, "Distance Fade");
    }

    private enum WorkflowMode
    {
      Specular,
      Metallic,
      Dielectric,
    }

    public enum BlendMode
    {
      Opaque,
      Cutout,
      Fade,
      Transparent,
    }

    public enum SmoothnessMapChannel
    {
      SpecularMetallicAlpha,
      AlbedoAlpha,
    }

    private static class Styles
    {
      public static GUIContent uvSetLabel = new("UV Set");
      public static GUIContent albedoText = new("Albedo", "Albedo (RGB) and Transparency (A)");
      public static GUIContent alphaCutoffText = new("Alpha Cutoff", "Threshold for alpha cutoff");
      public static GUIContent specularMapText = new("Specular", "Specular (RGB) and Smoothness (A)");
      public static GUIContent metallicMapText = new("Metallic", "Metallic (R) and Smoothness (A)");
      public static GUIContent smoothnessText = new("Smoothness", "Smoothness value");
      public static GUIContent smoothnessScaleText = new("Smoothness", "Smoothness scale factor");
      public static GUIContent smoothnessMapChannelText = new("Source", "Smoothness texture and channel");
      public static GUIContent highlightsText = new("Specular Highlights", "Specular Highlights");
      public static GUIContent reflectionsText = new("Reflections", "Glossy Reflections");
      public static GUIContent normalMapText = new("Normal Map", "Normal Map");
      public static GUIContent heightMapText = new("Height Map", "Height Map (G)");
      public static GUIContent occlusionText = new("Occlusion", "Occlusion (G)");
      public static GUIContent emissionText = new("Color", "Emission (RGB)");
      public static GUIContent detailMaskText = new("Detail Mask", "Mask for Secondary Maps (A)");
      public static GUIContent detailAlbedoText = new("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
      public static GUIContent detailNormalMapText = new("Normal Map", "Normal Map");
      public static string primaryMapsText = "Main Maps";
      public static string secondaryMapsText = "Secondary Maps";
      public static string forwardText = "Forward Rendering Options";
      public static string renderingMode = "Rendering Mode";
      public static string advancedText = "Advanced Options";

      public static GUIContent emissiveWarning =
        new(
          "Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");

      public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
    }
  }
}