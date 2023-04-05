using UnityEngine;

namespace VectorVanguard.Actors.Weapons
{
  public class SinWaveBullet : Bullet
  {
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequencyIncrement = 0.1f;
    
    
    private float _frequency;
    
    /// <summary>
    /// Bullet sine wave oscillation amplitude factor
    /// </summary>
    public float Amplitude
    {
      get => _amplitude;
      set => _amplitude = value;
    }
    
    /// <summary>
    /// The frequency increment of the sine wave
    /// </summary>
    public float FrequencyIncrement
    {
      get => _frequencyIncrement;
      set => _frequencyIncrement = value;
    }

    protected override void OnFire()
    {
      base.OnFire();
      _frequency = 0;
    }

    protected override void Move()
    {
      _xPosition = transform.position.x;
      _yPosition = transform.position.y;
      
      _xPosition += Mathf.Cos(Direction) * Speed * Time.deltaTime;
      _yPosition += Mathf.Sin(Direction) * Speed * Time.deltaTime;
      var beta = Direction + Mathf.PI / 2;
      _frequency += _frequencyIncrement;
      var distance = Amplitude * Mathf.Sin(_frequency);
      _xPosition += Mathf.Cos(beta) * distance;
      _yPosition += Mathf.Sin(beta) * distance;

      transform.position = new Vector2(_xPosition, _yPosition);
    }
  }
}