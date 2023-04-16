using UnityEngine;
using Random = UnityEngine.Random;

namespace VectorVanguard.VFX
{
  [RequireComponent(typeof(MeshRenderer))]
  public class NoiseMaterialController : MonoBehaviour
  {
    [Header("Components")] 
    [SerializeField] private Shader _shader;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("Noise Settings")] 
    [SerializeField] [Range(0f, 1f)] private float _opacity = 1f;

    [SerializeField] [Range(0f, 1f)] private float warpX = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float warpY = 0.5f;
    [SerializeField] [Range(0f, 5f)] private float reach = 3f;
    [SerializeField] private Color color = new(0.25f, 0.25f, 0.3f);
    [SerializeField] private Color color2 = new(0.7f, 0.7f, 0.7f);
    [SerializeField] private Vector2 offset;

    [Header("Randomize")] 
    [SerializeField] private int _randomizeSeed = 0;


    public MeshRenderer MeshRenderer
    {
      get => _meshRenderer;
      set => _meshRenderer = value;
    }
    
    public Shader Shader
    {
      get => _shader;
      set => _shader = value;
    }
    
    public void CreateMaterial()
    {
      var material = new Material(_shader)
      {
        name = "NoiseMaterial"
      };
#if UNITY_EDITOR
      if (!Application.isPlaying)
      {
        _meshRenderer.sharedMaterial = material;
      }
      else
      {
        _meshRenderer.material = material;
      }
#else
      _meshRenderer.material = material;
#endif
    }

    private void Awake()
    {
      if (_meshRenderer == null)
      {
        _meshRenderer = GetComponent<MeshRenderer>();
      }
    }

    private void Start()
    {
      if (_meshRenderer.sharedMaterial == null || _meshRenderer.sharedMaterial.shader != _shader)
      {
        CreateMaterial();
      }

      SetShaderData();
    }

    public void SetShaderData()
    {
      Material material;
#if UNITY_EDITOR
      if (!Application.isPlaying)
      {
        material = _meshRenderer.sharedMaterial;
      }
      else
      {
        material = _meshRenderer.material;
      }
#else
      material = _meshRenderer.material; 
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
      material.SetVector("_OffsetSeed", randomSeed);
      Random.state = prevState;
    }
  }
}