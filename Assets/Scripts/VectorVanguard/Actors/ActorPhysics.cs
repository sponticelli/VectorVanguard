using System;
using UnityEngine;

namespace VectorVanguard.Actors
{
  public class ActorPhysics : MonoBehaviour
  {
    
    [SerializeField] private LayerMask _collisionLayerMask;
    [SerializeField] private PolygonCollider2D _collider;
    
    private Actor _actor;
    
    private float _rotationForce;
    private Vector3 _linearForce;
    
    private bool _processed = false;
    
    private Transform _transform;
    
    public void Initialization(Actor actor)
    {
      _actor = actor;
      _transform = _actor.transform;
      _linearForce = Vector2.zero;
    }


    private void FixedUpdate()
    {
      _linearForce.z = 0;
      _transform.Rotate(0, 0, _rotationForce * Time.deltaTime);
      
      _transform.position += CheckForCollision() * Time.deltaTime;
      _processed = true;
    }

    private void LateUpdate()
    {
      if (_processed)
      {
        _rotationForce = 0;
        _linearForce = Vector2.zero;
        _processed = false;
      }
    }

    public void AddRotationForce(float rotation)
    {
      _rotationForce += rotation;
    }
    
    public void AddForce(Vector3 force)
    {
      _linearForce += force;
      
    }
    
    public Vector3 CheckForCollision()
    {
      // Calculate the distance based on _linearForce and Time.deltaTime
      var distance = (_linearForce * Time.deltaTime).magnitude;
      var collided = false;
      float bufferDistance = 0.01f; // Add a small buffer distance

      // Get the vertices of the polygon collider, assuming the Actor has a PolygonCollider2D component
      if (_collider == null)
      {
        var hit = Physics2D.Raycast(_transform.position, _linearForce.normalized, distance, _collisionLayerMask);
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
          var hit = Physics2D.Raycast(worldVertex, _linearForce.normalized, distance, _collisionLayerMask);

          if (hit.collider != null)
          {
            Debug.DrawLine(worldVertex, hit.point, Color.red);
            distance = Mathf.Min(distance, hit.distance - bufferDistance);
            collided = true;
          }
          else
          {
            Debug.DrawRay(worldVertex, _linearForce.normalized * distance, Color.yellow);
          }
        }
      }

      if (!collided) return _linearForce;
      
      // Calculate the new translation vector
      var newTranslation = _linearForce.normalized * distance / Time.deltaTime;
      return newTranslation;

    }
  }
  
  // A class that collect the abilites of an actor
}