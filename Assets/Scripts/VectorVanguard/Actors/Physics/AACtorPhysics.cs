using UnityEngine;

namespace VectorVanguard.Actors
{
  public abstract class AActorPhysics : MonoBehaviour, IActorPhysics
  {
    [SerializeField] protected LayerMask _collisionLayerMask;
    [SerializeField] protected PolygonCollider2D _collider;

    protected Actor _actor;

    public abstract void Initialization(Actor actor);
    public abstract void AddRotationForce(float rotation);
    public abstract void AddForce(Vector3 force);
    public abstract void AddExternalRotationForce(float rotation);
    public abstract void AddExternalForce(Vector3 force);
  }

}