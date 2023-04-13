using System;
using LiteNinja.Common;
using LiteNinja.Common.Helpers;
using UnityEngine;
using VectorVanguard.Actors;

namespace VectorVanguard.Levels
{
  /// <summary>
  /// An arrow that it is used to show the position of an actor when it is out of the screen.
  /// </summary>
  public class OutOfScreenIndicator : MonoBehaviour
  {
    [SerializeField] private Actor _target;
    [SerializeField] private float _distanceFromBorder = 0.1f;
    [SerializeField] private Actor _player;

    public Actor Actor
    {
      get => _target;
      set => _target = value;
    }

    private Vector3 _screenCenter = Vector3.zero;
    private Vector3 _horiziontalIntersectionPoint = Vector3.zero;
    private Vector3 _verticalIntersectionPoint = Vector3.zero;
    private Vector3 _direction = Vector3.zero;

    private bool _drawVertical = false;
    private bool _drawHorizontal = false;
    
    
    public void Initialization(Actor actor, Actor player)
    {
      _player = player;
      _target = actor;
      _screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }


    private void Update()
    {
      if (_target == null)
      {
        gameObject.SetActive(false);
        return;
      }

      //Camera center in world space with z = 0
      _direction = _target.transform.position - _player.transform.position;
      _direction.z = 0;
      _direction.Normalize();
      //find the angle of the direction
      var angle = -90 + Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle);
      
      // Convert the player position to screen space
      var targetScreenPosition = Camera.main.WorldToScreenPoint(_target.transform.position);
      var playerScreenPosition = Camera.main.WorldToScreenPoint(_player.transform.position);
      // Find the intersection of a line from target screen position with a screen border with the given direction _direction
      _horiziontalIntersectionPoint = Vector3.zero;
      _drawHorizontal = false;
      if (targetScreenPosition.x < playerScreenPosition.x)
      {
        _horiziontalIntersectionPoint = FindIntersection(playerScreenPosition, _direction, Side.Left);
      }
      else if (targetScreenPosition.x > playerScreenPosition.x)
      {
        _horiziontalIntersectionPoint = FindIntersection(playerScreenPosition, _direction, Side.Right);
      }
      
      _verticalIntersectionPoint = Vector3.zero;
      _drawVertical = false;
      if (targetScreenPosition.y < playerScreenPosition.y)
      {
        _verticalIntersectionPoint = FindIntersection(playerScreenPosition, _direction, Side.Bottom);
      }
      else if (targetScreenPosition.y > playerScreenPosition.y)
      {
        _verticalIntersectionPoint = FindIntersection(playerScreenPosition, _direction, Side.Top);
      }


      // Convert the intersection point to world space
      if (_verticalIntersectionPoint != Vector3.zero)
      {
        _verticalIntersectionPoint = Camera.main.ScreenToWorldPoint(_verticalIntersectionPoint);
        _verticalIntersectionPoint.z = 0;
        _drawVertical = true;
      }

      if (_verticalIntersectionPoint != Vector3.zero)
      {
        _horiziontalIntersectionPoint = Camera.main.ScreenToWorldPoint(_horiziontalIntersectionPoint);
        _horiziontalIntersectionPoint.z = 0;
        _drawHorizontal = true;
      }
    }

    private void OnDrawGizmos()
    {
      GizmoHelper.Arrow(transform.position, _direction, Color.red);
      if (_drawVertical) GizmoHelper.Point(_verticalIntersectionPoint, 1f, Color.yellow);
      if (_drawHorizontal) GizmoHelper.Point(_horiziontalIntersectionPoint, 1f, Color.yellow);
    }

  
    private enum  Side
    {
      Top,
      Bottom,
      Left,
      Right
    }
    
    private Vector3 FindIntersection(Vector3 screenPosition, Vector3 direction, Side side)
    {
      var intersectionPoint = Vector3.zero;
      var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
      
      //four corners of the screen
      Vector3 borderPoint = Vector3.zero;
      Vector3 borderDirection = Vector3.zero;
      switch (side)
      {
        case Side.Top:
          borderPoint = new Vector3(0, Screen.height, 0); 
          borderDirection = new Vector3(1, 0, 0);
          break;
        case Side.Bottom:
          borderPoint = new Vector3(0, 0, 0);
          borderDirection = new Vector3(1, 0, 0);
          break;
        case Side.Left:
          borderPoint = new Vector3(0, 0, 0);
          borderDirection = new Vector3(0, 1, 0);
          break;
        case Side.Right:
          borderPoint = new Vector3(Screen.width, 0, 0);
          borderDirection = new Vector3(0, 1, 0);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(side), side, null);
      }
      
      
      
      // Find the intersection with the top border
      var found = false;
      intersectionPoint = GetIntersectionPointCoordinates(screenPosition, screenPosition+direction, 
        borderPoint, borderPoint + borderDirection, out found);
      if (found)
      {
        // Check the intersection point is between top left and top right
        if (intersectionPoint.x >= 0 && intersectionPoint.x <= Screen.width && 
            intersectionPoint.y >= 0 && intersectionPoint.y <= Screen.height)
        {
          return intersectionPoint;
        }
      }
      
      return intersectionPoint;
    }

    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
      var tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);
 
      if (tmp == 0)
      {
        // No solution!
        found = false;
        return Vector2.zero;
      }
 
      var mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
 
      found = true;
 
      return new Vector2(
        B1.x + (B2.x - B1.x) * mu,
        B1.y + (B2.y - B1.y) * mu
      );
    }
    
    
  }
}