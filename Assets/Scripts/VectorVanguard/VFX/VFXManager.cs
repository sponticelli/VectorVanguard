using UnityEngine;
using VectorVanguard.Pools;
using VectorVanguard.Utils;

namespace VectorVanguard.VFX
{
  /// <summary>
  /// A Manager for all VFX in the game (impact effects, death effects, etc)
  /// </summary>
  public class VFXManager : MonoBehaviour
  {
    public void PlayImpactEffect(ImpactInfo impactInfo)
    {
      var impactEffect = PoolManager.Instance.GetObject(PoolTag.VFX_IMPACT, impactInfo.Position, 
        Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, -impactInfo.Direction, Vector3.forward))); ;
      impactEffect.SetActive(true);
      var particleSystemPlayer = impactEffect.GetComponent<ParticleSystemPlayer>();
      particleSystemPlayer?.Play();
    }
    
    public void PlayExplosionEffect(Vector3 position, PoolTag explosionTag)
    {
      var explosionEffect = PoolManager.Instance.GetObject(explosionTag, position); ;
      explosionEffect.SetActive(true);
      var particleSystemPlayer = explosionEffect.GetComponent<ParticleSystemPlayer>();
      particleSystemPlayer?.Play();
    }
    
    public void PlayAsteroidExplosionEffect(Vector3 position)
    {
      PlayExplosionEffect(position, PoolTag.VFX_ASTEROID_EXPLOSION);
    }
  }
}