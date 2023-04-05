using UnityEngine;

namespace VectorVanguard.Actors.Weapons
{
  public class MachineGunWeapon : APooledWeapon
  {
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private int _numberOfBullets = 5;
    [SerializeField] private float _cooldown = 0.5f;

    [SerializeField] private Transform _firePoint;
    
    private int _bulletsFired;
    private float _lastFireTime;


    public override bool CanFire
    {
      get
      {
        var timeSinceLastFire = Time.time - _lastFireTime;
        //If _bulletsFired is less than _numberOfBullets, we can fire if the time since the last fire is greater than the fire rate
        if (_bulletsFired < _numberOfBullets)
        {
          return timeSinceLastFire > _fireRate;
        }
        
        //After _numberOfBullets have been fired, we need to wait for the cooldown
        if (timeSinceLastFire > _cooldown)
        {
          _bulletsFired = 0;
          return true;
        }
        
        return false;
      }
    }
    
    public override bool Fire(float additionalForce = 0)
    {
      if (!base.Fire(additionalForce)) return false;
      
      _bulletsFired++;
      _lastFireTime = Time.time;
      return true;
    }
    
    protected override void SetupBullet(Bullet bullet)
    {
      base.SetupBullet(bullet);
      bullet.Speed = _bulletSpeed;
      bullet.Direction = _firePoint.rotation.eulerAngles.z * Mathf.Deg2Rad;
      bullet.transform.position = _firePoint.position;
      
    }
    
  }
}