using UnityEngine;

namespace VectorVanguard.Actors
{
  public interface  IActorPhysics
  {
    void AddRotationForce(float rotation);
    void AddForce(Vector3 force);
    void AddExternalRotationForce(float rotation);
    void AddExternalForce(Vector3 force);
  }
}