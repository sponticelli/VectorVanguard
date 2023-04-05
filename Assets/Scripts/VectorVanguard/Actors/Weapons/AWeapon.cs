using UnityEngine;
using UnityEngine.Events;

namespace VectorVanguard.Actors.Weapons
{
  public abstract class AWeapon : MonoBehaviour
  {
    [SerializeField] protected LayerMask _collisionMask;
    
    //UnityEvent can be used to call a function when the bullet is fired
    public UnityEvent OnFire;

    
    public abstract bool Fire(float additionalForce = 0);
    public virtual bool CanFire => true;

    protected virtual void SetupBullet(Bullet bullet)
    {
      bullet.CollisionMask = _collisionMask;
    }
  }
}