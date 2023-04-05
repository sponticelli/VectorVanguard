using UnityEngine;
using VectorVanguard.Pools;

namespace VectorVanguard.Actors.Weapons
{
  /// <summary>
  /// A weapon that uses a pool to spawn bullets
  /// </summary>
  public abstract class APooledWeapon : AWeapon
  {
    [SerializeField] private string _bulletPoolTag;
    
    public override bool Fire(float additionalForce = 0)
    {
      if (!CanFire) return false;
      var obj = PoolManager.Instance.GetObject(_bulletPoolTag);
      obj.transform.position = transform.position;
      obj.transform.rotation = transform.rotation;
      obj.gameObject.SetActive(true);
      
      var bullet = obj.GetComponent<Bullet>();
      if (!bullet) return false;
      SetupBullet(bullet);
      bullet.Speed += additionalForce;
      bullet.Fire();
      return true;
    }

    protected override void SetupBullet(Bullet bullet)
    {
      base.SetupBullet(bullet);
      bullet.DisableWhenDestroyed = true; // Disable the bullet when it is destroyed so it can be reused
    }
  }
}