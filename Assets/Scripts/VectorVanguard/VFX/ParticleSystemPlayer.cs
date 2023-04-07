using System.Collections;
using UnityEngine;

namespace VectorVanguard.VFX
{
  public class ParticleSystemPlayer : MonoBehaviour
  {
    [SerializeField] private ParticleSystem _particleSystem;
    
    public void Play() 
    {
      _particleSystem.Play();
      var particleDuration = _particleSystem.main.duration;
      Stop(particleDuration);

    }

    public void Stop(float particleDuration)
    {
      StartCoroutine(DelayedStop(particleDuration));
    }
    
    
    private IEnumerator DelayedStop(float particleDuration)
    {
      yield return new WaitForSeconds(particleDuration);
      _particleSystem.Stop();
      gameObject.SetActive(false);
    }
  }
}