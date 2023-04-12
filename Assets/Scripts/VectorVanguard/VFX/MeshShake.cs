using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VectorVanguard.VFX
{
  [RequireComponent(typeof(MeshFilter))]
  public class MeshShake : MonoBehaviour
  {
    
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private float minDuration;
    [SerializeField] private float maxDuration;
    [SerializeField] private float minMagnitude;
    [SerializeField] private float maxMagnitude;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    
    
    private Mesh _originalMesh;
    private Mesh _workingMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    
    private Coroutine _shakeCoroutine;
    private bool _canShake = false;

    private void Start()
    {
      if (meshFilter == null)
      {
        meshFilter = GetComponent<MeshFilter>();
      }

      _originalMesh = meshFilter.mesh;
      _workingMesh = Instantiate(_originalMesh);
      _originalVertices = _originalMesh.vertices;
      _displacedVertices = new Vector3[_originalVertices.Length];

      for (var i = 0; i < _originalVertices.Length; i++)
      {
        _displacedVertices[i] = _originalVertices[i];
      }

      _canShake = true;
    }

    private void OnEnable()
    {
      _canShake = true;
    }
    
    private void OnDisable()
    {
      _canShake = false;
      if (_originalVertices != null)
      {
        _workingMesh.vertices = _originalVertices;
      }
    }


    public void Shake()
    {
      
      Shake(Random.Range(minDuration, maxDuration), 
        Random.Range(minMagnitude, maxMagnitude), 
        Random.Range(minSpeed, maxSpeed));
    }
    
    public void Shake(float duration, float magnitude, float speed)
    {
      if (_shakeCoroutine != null)
      {
        StopCoroutine(_shakeCoroutine);
      }
      if (!_canShake) return;
      _shakeCoroutine = StartCoroutine(ApplyShake(duration, magnitude, speed));
    }

    private IEnumerator ApplyShake(float duration, float magnitude, float speed)
    {
      var startTime = Time.time;

      while (Time.time < startTime + duration)
      {
        if (!_canShake) break;
        for (var i = 0; i < _originalVertices.Length; i++)
        {
          var offset = Mathf.Sin(Time.time * speed) * magnitude;
          var randomOffset = Random.insideUnitCircle * offset;
          _displacedVertices[i].x = _originalVertices[i].x + randomOffset.x;
          _displacedVertices[i].y = _originalVertices[i].y + randomOffset.y;
        }

        _workingMesh.vertices = _displacedVertices;
        
        yield return null;
      }

      // Reset vertices to original positions after the shake is complete
      _workingMesh.vertices = _originalVertices;
    }
  }
}