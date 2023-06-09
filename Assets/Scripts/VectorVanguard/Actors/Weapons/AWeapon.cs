using UnityEngine;
using UnityEngine.Events;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors.Weapons
{
  public abstract class AWeapon : MonoBehaviour, IFactionable
  {
    [SerializeField] protected EntityFaction _faction;
    [SerializeField] protected LayerMask _collisionMask;
    
    //UnityEvent can be used to call a function when the bullet is fired
    public UnityEvent OnFire;


    public EntityFaction Faction
    {
      get => _faction;
      set => _faction = value;
    }
    
    public abstract bool Fire(float additionalForce = 0);
    public virtual bool CanFire => true;

    protected virtual void SetupBullet(Bullet bullet)
    {
      bullet.CollisionMask = _collisionMask;
      bullet.Faction = _faction;
    }
  }
}