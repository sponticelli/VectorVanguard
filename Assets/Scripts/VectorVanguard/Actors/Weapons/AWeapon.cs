using UnityEngine;

namespace VectorVanguard.Actors.Weapons
{
  public abstract class AWeapon : MonoBehaviour
  {
    [SerializeField] protected LayerMask _collisionMask;
    public abstract bool Fire(float additionalForce = 0);
    public virtual bool CanFire => true;

    protected virtual void SetupBullet(Bullet bullet)
    {
      bullet.CollisionMask = _collisionMask;
    }
  }
}