using UnityEngine;

namespace VectorVanguard.VFX
{
  public class Starfield : MonoBehaviour
  {
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private int _maxStars = 100;
    [SerializeField] private float _minStarSize = 0.1f;
    [SerializeField] private float _maxStarSize = 0.5f;
    [SerializeField] private float _fieldWidth = 20f;
    [SerializeField] private float _fieldHeight = 25f;
    [SerializeField] private bool _grayScale = false;

    private float _xOffset;
    private float _yOffset;
    private ParticleSystem.Particle[] _stars;


    void Awake()
    {
      _stars = new ParticleSystem.Particle[_maxStars];
      if (_particleSystem == null)
      {
        _particleSystem = GetComponent<ParticleSystem>();
      }

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

    private Vector3 GetRandomInRectangle(float width, float height)
    {
      var x = Random.Range(0, width);
      var y = Random.Range(0, height);
      return new Vector3(x - _xOffset, y - _yOffset, 0);
    }
  }
}