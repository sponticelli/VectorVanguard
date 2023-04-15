using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VectorVanguard.VFX
{
  [RequireComponent(typeof(MeshRenderer))]
  public class NoiseMaterialController : MonoBehaviour
  {
    [SerializeField] private Shader _shader;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("Noise Settings")] [SerializeField] [Range(0f, 1f)]
    private float _opacity = 1f;

    [SerializeField] [Range(0f, 1f)] private float warpX = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float warpY = 0.5f;
    [SerializeField] [Range(0f, 5f)] private float reach = 3f;
    [SerializeField] private Color color = new(0.25f, 0.25f, 0.3f);
    [SerializeField] private Color color2 = new(0.7f, 0.7f, 0.7f);
    [SerializeField] private Vector2 offset;

    [Header("Randomize")] [SerializeField] private int _randomizeSeed = 0;

    private void CreateMaterial()
    {
      var material = new Material(_shader)
      {
        name = "NoiseMaterial"
      };
      material.SetFloat("_Opacity", _opacity);
      material.SetFloat("_WarpX", warpX);
      material.SetFloat("_WarpY", warpY);
      material.SetFloat("_Reach", reach);
      material.SetColor("_Color", color);
      material.SetColor("_Color2", color2);
      material.SetVector("_Offset", offset);
      _meshRenderer.material = material;
    }


    private void OnValidate()
    {
      if (_meshRenderer == null)
      {
        _meshRenderer = GetComponent<MeshRenderer>();
      }

      if (_meshRenderer.sharedMaterial == null || _meshRenderer.sharedMaterial.shader != _shader)
      {
        CreateMaterial();
      }

      SetShaderData();
    }

    private void SetShaderData()
    {
      var material = _meshRenderer.material;
#if UNITY_EDITOR
      if (!Application.isPlaying)
      {
        material = _meshRenderer.sharedMaterial;
      }
#endif

      material.SetFloat("_Opacity", _opacity);
      material.SetFloat("_WarpX", warpX);
      material.SetFloat("_WarpY", warpY);
      material.SetFloat("_Reach", reach);
      material.SetColor("_Color", color);
      material.SetColor("_Color2", color2);
      material.SetVector("_Offset", offset);
      var prevState = Random.state;
      Random.InitState(_randomizeSeed);
      var randomSeed = new Vector4(
        Random.Range(-1000f, 1000f), 
        Random.Range(-1000f, 1000f),
        Random.Range(-1000f, 1000f), 
        Random.Range(-1000f, 1000f));
      _meshRenderer.material.SetVector("_OffsetSeed", randomSeed);
      Random.state = prevState;
    }
  }
}