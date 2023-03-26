using System;
using UnityEngine;

namespace VectorVanguard.Actors
{
  public class ActorPhysics :  AActorPhysics
  {
    
    
    
    private float _internalRotationForce;
    private Vector3 _internalLinearForce;
    private float _externalRotationForce;
    private Vector3 _externalLinearForce;
    
    
    private bool _processed = false;
    
    private Transform _transform;
    
    public override void Initialization(Actor actor)
    {
      _actor = actor;
      _transform = _actor.transform;
      _internalLinearForce = Vector2.zero;
      _internalRotationForce = 0;
      _externalLinearForce = Vector2.zero;
      _externalRotationForce = 0;
    }


    private void FixedUpdate()
    {
      _internalLinearForce.z = 0;
      _externalLinearForce.z = 0;
      _transform.Rotate(0, 0, (_internalRotationForce+_externalRotationForce) * Time.deltaTime);
      
      _transform.position += CheckForCollision() * Time.deltaTime;
      _processed = true;
    }

    private void LateUpdate()
    {
      if (!_processed) return;
      _internalLinearForce = Vector3.zero;
      _internalRotationForce = 0;
      _externalLinearForce = Vector3.zero;
      _externalRotationForce = 0;
        
      _processed = false;
    }

    public override void AddRotationForce(float rotation)
    {
      _internalRotationForce += rotation;
    }
    
    public override void AddForce(Vector3 force)
    {
      _internalLinearForce += force;
    }
    
    public override  void AddExternalRotationForce(float rotation)
    {
      _externalRotationForce += rotation;
    }
    
    public override void AddExternalForce(Vector3 force)
    {
      _externalLinearForce += force;
    }

    private Vector3 CheckForCollision()
    {
      // Calculate the distance based on _linearForce and Time.deltaTime
      var totalForce = _internalLinearForce + _externalLinearForce;
      var totalForceNormalized = totalForce.normalized;
      var distance = ((totalForce) * Time.deltaTime).magnitude;
      var collided = false;
      var bufferDistance = 0.01f; // Add a small buffer distance

      // Get the vertices of the polygon collider, assuming the Actor has a PolygonCollider2D component
      if (_collider == null)
      {
        var hit = Physics2D.Raycast(_transform.position, totalForce.normalized, distance, _collisionLayerMask);
        if (hit.collider != null)
        {
          distance = hit.distance - bufferDistance;
          collided = true;
        }
      }
      else
      {
        var vertices = _collider.points;

        // Perform raycasts from each vertex in the direction of movement
        foreach (var vertex in vertices)
        {
          Vector2 worldVertex = _transform.TransformPoint(vertex);
          var hit = Physics2D.Raycast(worldVertex, totalForceNormalized, distance, _collisionLayerMask);

          if (hit.collider != null)
          {
            Debug.DrawLine(worldVertex, hit.point, Color.red);
            distance = Mathf.Min(distance, hit.distance - bufferDistance);
            collided = true;
          }
          else
          {
            Debug.DrawRay(worldVertex, totalForceNormalized.normalized * distance, Color.yellow);
          }
        }
      }

      if (!collided) return totalForce;
      
      // Calculate the new translation vector
      var newTranslation = totalForceNormalized * distance / Time.deltaTime;
      return newTranslation;

    }
  }
}