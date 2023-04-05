using System;
using UnityEngine;
using VectorVanguard.Utils;

namespace VectorVanguard.Actors.Weapons
{
  [RequireComponent(typeof(Collider2D))]
  public class Bullet : MonoBehaviour
  {
    [SerializeField] protected LayerMask _collisionMask;
    [SerializeField] protected float _maxLifetime = 5;
    [SerializeField] protected float _speed = 10;
    [SerializeField] protected bool _destroyIfNotVisible = true;
    [SerializeField] protected bool _disableWhenDestroyed = true;
    [SerializeField] protected float _damage = 1;
    
    /// <summary>
    /// The speed at which the bullet will travel at
    /// </summary>
    public float Speed
    {
      get => _speed;
      set => _speed = value;
    }

    /// <summary>
    /// The direction bullet is travelling in radians.
    /// </summary>
    public float Direction { get; set; }

    /// <summary>
    /// The layers that the bullet can collide with
    /// </summary>
    public LayerMask CollisionMask
    {
      get => _collisionMask;
      set => _collisionMask = value;
    }
    

    /// <summary>
    ///  Max lifetime of the bullet in seconds
    /// </summary>
    public float MaxLifetime
    {
      get => _maxLifetime;
      set => _maxLifetime = value;
    }
    
    /// <summary>
    /// Destroy the bullet if it is not visible on the screen
    /// </summary>
    public bool DestroyIfNotVisible
    {
      get => _destroyIfNotVisible;
      set => _destroyIfNotVisible = value;
    }
    
    /// <summary>
    /// Disable the bullet when it is destroyed
    /// </summary>
    public bool DisableWhenDestroyed
    {
      get => _disableWhenDestroyed;
      set => _disableWhenDestroyed = value;
    }
    
    
    protected float _xPosition;
    protected float _yPosition;
    private float _timeAlive;
    
    private Renderer _renderer;

    public void Fire()
    {
      // When the bullet is enabled, ensure it faces the correct direction
      transform.eulerAngles = new Vector3(0.0f, 0.0f, Direction * Mathf.Rad2Deg);
      _timeAlive = 0;
      if (_renderer == null) _renderer = GetComponent<Renderer>();
      OnFire();
    }
    
    private void Update()
    {
      _timeAlive += Time.deltaTime;
      Move();
      Lifetime();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      //Check if the bullet is colliding with a layer that is in the collision mask
      if (((1 << col.gameObject.layer) & CollisionMask) == 0) return;
      OnCollide(col);
      OnAfterCollide(col);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
      //Check if the bullet is colliding with a layer that is in the collision mask
      if (((1 << col.gameObject.layer) & CollisionMask) == 0) return;
      OnCollide(col.collider);
      OnAfterCollide(col.collider);
    }

    /// <summary>
    /// Called when the bullet collides with something
    /// </summary>
    /// <param name="col"></param>
    protected virtual void OnCollide(Collider2D col)
    {
      var damageable = col.GetComponent<IDamageable>();
      if (damageable != null)
      {
        damageable.TakeDamage(_damage);
      }
    }

    /// <summary>
    /// Called after the bullet has collided with something
    /// </summary>
    /// <param name="col"></param>
    protected virtual void OnAfterCollide(Collider2D col)
    {
      Destroy(); //disable so it can be reused
    }

    protected virtual void Move()
    {
      _xPosition = transform.position.x;
      _yPosition = transform.position.y;

      _xPosition += Speed * Mathf.Cos(Direction) * Time.deltaTime;
      _yPosition += Speed * Mathf.Sin(Direction) * Time.deltaTime;

      transform.position = new Vector3(_xPosition, _yPosition, 0);
    }
    
    protected virtual void Lifetime()
    {
      if (_timeAlive > MaxLifetime)
      {
        Destroy();
      }
      
      if (_destroyIfNotVisible && !_renderer.isVisible)
      {
        Destroy();
      }
    }
    
    protected virtual void OnFire()
    {
      
    }


    private void Destroy()
    {
      if (_disableWhenDestroyed)
      {
        gameObject.SetActive(false);
      }
      else
      {
        Destroy(gameObject);  
      }
    }
    
  }
}