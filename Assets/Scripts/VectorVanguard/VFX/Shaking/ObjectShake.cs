using System.Collections;
using UnityEngine;

namespace VectorVanguard.VFX
{
  public class ObjectShake : MonoBehaviour
  {
    [SerializeField] private float minDuration = 0.15f;
    [SerializeField] private float maxDuration = 0.25f;
    [SerializeField] private float minMagnitude = 0.2f;
    [SerializeField] private float maxMagnitude = 0.3f;
    [SerializeField] private float minTimeBetweenShakes = 0.01f;
    [SerializeField] private float maxTimeBetweenShakes = 0.05f;
    [SerializeField] private float _minAngle;
    [SerializeField] private float _maxAngle = 45f;
    [SerializeField] private bool _smoothShake = true;
    
    private bool _isShaking;
    private float _duration;
    
    
    public void Shake()
    {
      var duration = Random.Range(minDuration, maxDuration);
      var magnitude = Random.Range(minMagnitude, maxMagnitude);
      var timeBetweenShakes = Random.Range(minTimeBetweenShakes, maxTimeBetweenShakes);
      var angle = Random.Range(_minAngle, _maxAngle);
      if (!gameObject.activeInHierarchy) return;
      Shake(magnitude, angle, duration,timeBetweenShakes, _smoothShake);
    }
    
    public void Shake(float amount, float angle, float duration, float timeBetweenShakes = 0.1f, bool smoothShake = true)
    {
      if (_isShaking)
      {
        _duration += duration;
        return;
      }
      StartCoroutine(ApplyShake(amount, angle, duration, timeBetweenShakes, smoothShake));
    }

    private IEnumerator ApplyShake(float amount, float angle, float duration, float timeBetweenShakes, bool smoothShake = true)
    {
      _isShaking = true;
      var originalPos = transform.localPosition;
      var originalRot = transform.localRotation.z;
      var counter = 0;
      var direction = 1;
      _duration = duration;
      while (_duration > 0)
      {
        counter= _smoothShake ? counter+1 :  1;
        var xOffset = Random.Range(-1f, 1f) * (amount / (float)counter);
        var yOffset = Random.Range(-1f, 1f) * (amount / (float)counter);
        var zAngle = direction * angle * _duration;
        transform.localRotation = Quaternion.Euler(0, 0, zAngle);
        transform.localPosition = Vector3.Lerp(transform.localPosition,
          new Vector3(originalPos.x + xOffset, originalPos.y + yOffset, originalPos.z), 0.5f);
        _duration -= Time.deltaTime;
        direction *= -1;
        yield return new WaitForSeconds(timeBetweenShakes);
      }

      transform.localPosition = originalPos;
      transform.localRotation = Quaternion.Euler(0, 0, originalRot);
      _isShaking = false;
    }
  }
}

