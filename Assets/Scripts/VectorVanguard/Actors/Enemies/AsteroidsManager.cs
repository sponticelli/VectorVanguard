using System.Collections.Generic;
using UnityEngine;
using VectorVanguard.Pools;

namespace VectorVanguard.Actors.Enemies
{
  public class AsteroidsManager : MonoBehaviour
  {
    [Header("Pool")]
    [SerializeField] private string _smallAsteroidTag;
    [SerializeField] private string _mediumAsteroidTag;
    [SerializeField] private string _bigAsteroidTag;
    
    [Header("Asteroids Radius")]
    [SerializeField] private float _smallAsteroidRadius;
    [SerializeField] private float _mediumAsteroidRadius;
    [SerializeField] private float _bigAsteroidRadius;
    
    [Header("Asteroids creation")]
    [SerializeField] private int _mediumAsteroidsFromBigAsteroid = 2;
    [SerializeField] private int _smallAsteroidsFromMediumAsteroid = 2;
    
    public void OnSmallAsteroidDestroyed(Vector3 position)
    {
      
    }
    
    public void OnMediumAsteroidDestroyed(Vector3 position)
    {
      var positions = GetPositionsOfCircles(position, _mediumAsteroidRadius, _smallAsteroidRadius, _smallAsteroidsFromMediumAsteroid);
      foreach (var t in positions)
      {
        var asteroid = PoolManager.Instance.GetObject(_smallAsteroidTag, t, Quaternion.Euler(0, 0, Random.Range(0, 360)));
      }
    }
    
    public void OnBigAsteroidDestroyed(Vector3 position)
    {
      var positions = GetPositionsOfCircles(position, _bigAsteroidRadius, _mediumAsteroidRadius, _mediumAsteroidsFromBigAsteroid);
      foreach (var t in positions)
      {
        var asteroid = PoolManager.Instance.GetObject(_mediumAsteroidTag, t, Quaternion.Euler(0, 0, Random.Range(0, 360)));
      }
    }
    
    
    
    /// <summary>
    /// Given a center position and the radius of circle, the radius of 2 smaller circles and the number of smaller circles
    /// Returns the positions of the smaller circles
    /// </summary>
    /// <param name="centerPosition"></param>
    /// <param name="radius"></param>
    /// <param name="smallerRadius"></param>
    /// <param name="numberOfCircles"></param>
    /// <returns>list of Vector3</returns>
    private static IEnumerable<Vector3> GetPositionsOfCircles(Vector3 centerPosition, float radius, float smallerRadius, int numberOfCircles)
    {
      var positions = new Vector3[numberOfCircles];
      for (var i = 0; i < numberOfCircles; i++)
      {
        positions[i] = centerPosition + (Vector3)Random.insideUnitCircle * radius;
      }
      
      // check that the small asteroids are not colliding with each other
      for (var i = 0; i < numberOfCircles; i++)
      {
        for (var j = 0; j < numberOfCircles; j++)
        {
          if (i == j) continue;
          while (Vector3.Distance(positions[i], positions[j]) < smallerRadius * 2)
          {
            positions[i] = centerPosition + (Vector3)Random.insideUnitCircle * radius;
          }
        }
      }
      return positions;
    }
  }
}