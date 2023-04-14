using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace VectorVanguard.Cameras
{
  [RequireComponent(typeof(Camera))]
  [ExecuteInEditMode]
  public class BloomVFX : MonoBehaviour
  {


    [Tooltip("How many iterations are used to draw the effect")] 
    [SerializeField] [Range(2, 9)]
    private int _numOfIterations = 5;

    [Tooltip("How much the effect diffuses the image")] 
    [SerializeField] [Range(0, 1)]
    private float _diffusion = 1f;

    [Tooltip("The color of the effect")] 
    [SerializeField]
    private Color _color = Color.white;

    [Tooltip("How much the effect is applied")] 
    [SerializeField]
    private float _amount = 1f;

    [Tooltip(
      "The threshold at which the effect is applied. The lower the value, the more the bloom is applied to the darker areas of the screen")]
    [SerializeField]
    private float _threshold = 1.0f;

    [Tooltip("The softness of the effect")] 
    [SerializeField] [Range(0, 1)] 
    private float _softness;

    [Tooltip("The material with the BloomVFX shader")] [SerializeField]
    private Material material;

    private static readonly int BlurAmountString = Shader.PropertyToID("blur_amount");
    private static readonly int BloomColorString = Shader.PropertyToID("bloom_color");
    private static readonly int BloomDataString = Shader.PropertyToID("bloom_data");
    private static readonly int BloomTexString = Shader.PropertyToID("bloom_tex");
    private const string RGBMKeyword = "_USE_RGBM";
    private const int ShaderPass0 = 0;
    private const int ShaderPass1 = 1;
    private const int ShaderPass2 = 2;
    private const int ShaderPass3 = 3;


    private float _knee;
    private RenderTextureFormat _format;
    private bool _rgbm;
    private Vector4 _bloomData;
    private RenderTextureDescriptor _opaqueDesc;
    private readonly Vector3 _lm = new Vector3(0.2126f, 0.7152f, 0.0722f);
    
    private static readonly int SourceTexString = Shader.PropertyToID("_SourceTex");


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (_diffusion == 0 && _amount == 0)
      {
        Graphics.Blit(source, destination);
        return;
      }

      var c = _color.linear;
      var l = Vector3.Dot(new Vector3(c.r, c.g, c.b), _lm);
      var color = l > 0f ? c * (1f / l) : Color.white;
      var threshold = Mathf.GammaToLinearSpace(_threshold);
      _knee = threshold * _softness;
      _bloomData = new Vector4(threshold, threshold - _knee, 2f * _knee, 1f / (4f * _knee + 0.0001f));
      _rgbm = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
      _format = !_rgbm ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32;

      material.SetFloat(BlurAmountString, Mathf.Lerp(0.05f, 0.95f, _diffusion));
      material.SetColor(BloomColorString, color * _amount);
      material.SetVector(BloomDataString, _bloomData);

      switch (_rgbm)
      {
        case true when !material.IsKeywordEnabled(RGBMKeyword):
          material.EnableKeyword(RGBMKeyword);
          break;
        case false when material.IsKeywordEnabled(RGBMKeyword):
          material.DisableKeyword(RGBMKeyword);
          break;
      }

      _opaqueDesc = XRSettings.enabled
        ? XRSettings.eyeTextureDesc
        : new RenderTextureDescriptor(Screen.width, Screen.height, _format, 0);
      _opaqueDesc.autoGenerateMips = false;
      _opaqueDesc.useMipMap = false;
      _opaqueDesc.msaaSamples = 1;


      var bloomTemp = new RenderTexture[_numOfIterations];
      var bloomTemp1 = new RenderTexture[_numOfIterations];
      
      var bloomTextures = new RenderTexture[_numOfIterations * 2];

      AllocTemporaryTextures(bloomTextures);
      ApplyPasses(source, destination, bloomTextures);
      ReleaseTemporaryTextures(bloomTextures);
    }
    
    /// <summary>
    ///  Release the temporary textures
    /// </summary>
    /// <param name="bloomTemp"></param>
    /// <param name="bloomTemp1"></param>
    private void ReleaseTemporaryTextures(IReadOnlyList<RenderTexture> bloomTextures)
    {
      for (var i = 0; i < _numOfIterations; i++)
      {
        RenderTexture.ReleaseTemporary(bloomTextures[i]);
        RenderTexture.ReleaseTemporary(bloomTextures[i + _numOfIterations]);
      }
    }

    private void AllocTemporaryTextures(IList<RenderTexture> bloomTextures)
    {
      for (var i = 0; i < _numOfIterations; i++)
      {
        _opaqueDesc.width = Mathf.Max(1, _opaqueDesc.width >> 1);
        _opaqueDesc.height = Mathf.Max(1, _opaqueDesc.height >> 1);
        bloomTextures[i] = RenderTexture.GetTemporary(_opaqueDesc);
        bloomTextures[i + _numOfIterations] = RenderTexture.GetTemporary(_opaqueDesc);
      }
    }

    private void ApplyPasses(Texture source, RenderTexture destination, IReadOnlyList<RenderTexture> bloomTextures)
    {
      
      Graphics.Blit(source, bloomTextures[0], material, ShaderPass0);

      for (var i = 0; i < _numOfIterations - 1; i++)
      {
        Graphics.Blit(bloomTextures[i], bloomTextures[i + 1], material, ShaderPass1);
      }

      for (var i = _numOfIterations - 2; i >= 0; i--)
      {
        material.SetTexture(BloomTexString, i == _numOfIterations - 2 ? bloomTextures[i + 1] : 
          bloomTextures[i + _numOfIterations + 1]);
        Graphics.Blit(bloomTextures[i], bloomTextures[i + _numOfIterations], material, ShaderPass2);
      }

      material.SetTexture(BloomTexString, bloomTextures[_numOfIterations]);
      Graphics.Blit(source, destination, material, ShaderPass3);
    }

  }
}