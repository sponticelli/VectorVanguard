using System;
using UnityEngine;

namespace VectorVanguard.Actors
{
  public class ActorPhysics : MonoBehaviour
  {
    private Actor _actor;
    
    private float _rotationFactor;
    
    private bool _processed = false;
    
    
    public void Initialization(Actor actor)
    {
      _actor = actor;
    }


    private void FixedUpdate()
    {
      _actor.transform.Rotate(0, 0, _rotationFactor * Time.fixedTime, Space.World);
      _processed = true;
    }

    private void LateUpdate()
    {
      if (_processed)
      {
        _rotationFactor = 0;
        _processed = false;
      }
    }

    public void AddRotation(float rotation)
    {
      _rotationFactor += rotation;
    }
  }
}