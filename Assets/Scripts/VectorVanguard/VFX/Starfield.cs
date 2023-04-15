using UnityEngine;
using VectorVanguard.Levels;

namespace VectorVanguard.VFX
{
  [RequireComponent(typeof(ParticleSystem))]
  public class Starfield : MonoBehaviour
  {
    [SerializeField] private ActiveActorsSO _activeActors;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _cameraTransform;
    
    [SerializeField] private float _parallaxFactor = 0.5f;
    
    [SerializeField] private int _maxStars = 100;
    [SerializeField] private float _minStarSize = 0.1f;
    [SerializeField] private float _maxStarSize = 0.5f;
    
    [SerializeField] private bool _grayScale = false;

    private float _xOffset;
    private float _yOffset;
    private float _fieldWidth;
    private float _fieldHeight;
    private ParticleSystem.Particle[] _stars;


    void Awake()
    {
      _stars = new ParticleSystem.Particle[_maxStars];
      if (_particleSystem == null)
      {
        _particleSystem = GetComponent<ParticleSystem>();
      }
      
      var camera = _activeActors.MainCamera;
      _cameraTransform = camera.transform;
      //get screen size
      var screenSize = new Vector2(camera.pixelWidth, camera.pixelHeight);
      //get world size
      var worldSize = camera.ScreenToWorldPoint(screenSize);
      _fieldWidth = worldSize.x * 2;
      _fieldHeight = worldSize.y * 2;


      _xOffset = _fieldWidth * 0.5f; // Offset the coordinates to distribute the spread
      _yOffset = _fieldHeight * 0.5f; // around the object's center

      for (int i = 0; i < _maxStars; i++)
      {
        var randSize = Random.Range(_minStarSize, _maxStarSize); // Randomize star size within parameters
        var scaledColor =
          _grayScale ? randSize / _maxStarSize : 1f; // If coloration is desired, color based on size

        _stars[i].position = GetRandomInRectangle(_fieldWidth, _fieldHeight) + transform.position;
        _stars[i].startSize = _minStarSize * randSize;
        _stars[i].startColor = new Color(scaledColor, scaledColor, scaledColor, 1f);
      }

      _particleSystem.SetParticles(_stars, _stars.Length); // Write data to the particle system
    }

    private void Update ()
    {
      var newPosition = _cameraTransform.position * _parallaxFactor;
      newPosition.z = transform.position.z;
      transform.position = newPosition;
      for ( var i=0; i<_maxStars; i++ )
      {
        var pos = _stars[ i ].position + transform.position ;

        if ( pos.x < ( _cameraTransform.position.x - _xOffset ) )
        {
          pos.x += _fieldWidth;
        }
        else if ( pos.x > ( _cameraTransform.position.x + _xOffset ) )
        {
          pos.x -= _fieldWidth;
        }

        if ( pos.y < ( _cameraTransform.position.y - _yOffset ) )
        {
          pos.y += _fieldHeight;
        }
        else if ( pos.y > ( _cameraTransform.position.y + _yOffset ) )
        {
          pos.y -= _fieldHeight;
        }

        _stars[ i ].position = pos - transform.position;
      }
      _particleSystem.SetParticles( _stars, _stars.Length );

    }

    private Vector3 GetRandomInRectangle(float width, float height)
    {
      var x = Random.Range(0, width);
      var y = Random.Range(0, height);
      return new Vector3(x - _xOffset, y - _yOffset, 0);
    }
  }
  
  
}