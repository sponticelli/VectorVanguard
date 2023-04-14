namespace VectorVanguard.Utils
{
  [System.Flags]
  public enum EntityType 
  {
    None = 0,
    Player = 1,
    Asteroid = 2,
    Enemy = 4,
    Bullet = 8,
    PowerUp = 16,
  }
}