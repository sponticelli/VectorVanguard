using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiteNinja.PathMaster._2D
{
  /// <summary>
  /// The MonoPath2D class is used to generate and manipulate a 2D path within the Unity game engine.
  /// </summary>
  public class MonoPath2D : MonoBehaviour
  {
    [SerializeField, HideInInspector] private Path2D _path;
    [SerializeField, HideInInspector] private NodeType _defaultControlType = NodeType.SMOOTH;
    [SerializeField, HideInInspector] private float _spacing = 0.1f;
    [SerializeField, HideInInspector] private Vector2[] _nodes;

    private readonly List<float> _distances = new();
    private bool _shouldInitialize = true;

#region Getters and Setters
    
    /// <summary>
    /// Gets or sets the current Path2D object.
    /// </summary>
    public Path2D Path
    {
      get => _path;
      set => _path = value;
    }
    
    /// <summary>
    /// Gets or sets the default NodeType for the path's control points.
    /// </summary>
    public NodeType DefaultControlType
    {
      get => _defaultControlType;
      set
      {
        _defaultControlType = value;
        Initialize();
      }
    }
    
    /// <summary>
    /// Gets or sets the spacing between nodes in the path.
    /// </summary>
    public float Spacing
    {
      get => _spacing;
      set
      {
        _spacing = value;
        Initialize();
      }
    }
    
    /// <summary>
    /// Gets or sets the nodes in the path.
    /// </summary>
    public Vector2[] Nodes
    {
      get => _nodes;
      set => _nodes = value;
    }
    
    /// <summary>
    /// Gets the length of the path.
    /// </summary>
    public float Length { get; private set; }

#endregion
    
    /// <summary>
    /// Creates a new Path2D object.
    /// </summary>
    public void CreatePath()
    {
      _path = new Path2D(new Vector2(), _defaultControlType);
    }
    
    /// <summary>
    /// Resets the current path.
    /// </summary>
    public void Reset()
    {
      CreatePath();
      Initialize();
    }
    
    /// <summary>
    /// Initializes the path by calculating equidistant nodes and the path's total length.
    /// </summary>
    public void Initialize()
    {
      _nodes = _path.GetEquidistantNodes(_spacing);
      Length = 0;
      _distances.Clear();
      _distances.Add(Length);
      for (var i = 1; i < _nodes.Length; i++)
      {
        Length += Vector2.Distance(_nodes[i], _nodes[i - 1]);
        _distances.Add(Length);
      }

    }

    /// <summary>
    /// Finds the closest point on the path to a given value by performing a binary search on the distances of the nodes.
    /// </summary>
    /// <param name="from">The starting index for the search.</param>
    /// <param name="to">The ending index for the search.</param>
    /// <param name="val">The value for which to find the closest point on the path.</param>
    /// <returns>A Vector2 representing the closest point on the path to the given value.</returns>

    private Vector2 FindClosest(int from, int to, float val)
    {
      while (true)
      {
        if (from == to - 1)
        {
          var t = (val - _distances[from]) / (_distances[to] - _distances[from]);
          return Vector2.Lerp(_nodes[from], _nodes[to], t);
        }

        var i = from + (to - from) / 2;
        if (val > _distances[i])
        {
          from = i;
          continue;
        }

        to = i;
      }
    }
    
    /// <summary>
    /// Returns a position on the path at a specified distance along the path from the start.
    /// If the input distance exceeds the path length, it returns the last point on the path or loops to the start of
    /// the path based on the 'loop' parameter.
    /// </summary>
    /// <param name="distance">The distance along the path at which to get the position.</param>
    /// <param name="loop">Optional parameter which if set to true, loops to the start of the path if the distance is
    /// greater than the path length. Defaults to false.</param>
    /// <param name="space">Optional parameter to specify if the coordinates should be in local or world space.
    /// Defaults to world space.</param>
    /// <returns>A Vector2 point at the specified distance along the path.</returns>
    public Vector2 GetPositionByDistance(float distance, bool loop = false, Space space = Space.World)
    {
      if (_shouldInitialize)
      {
        _shouldInitialize = false;
        Initialize();
      }

      var currentDistance = distance;
      if (distance > Length)
      {
        if (loop)
        {
          currentDistance = ((distance / Length) - (int)(distance / Length)) * Length;
        }
        else
        {
          currentDistance = Length;
        }
      }

      var ret = FindClosest(0, _nodes.Length - 1, currentDistance);

      if (space == Space.World)
        ret += (Vector2)transform.position;

      return ret;
    }
    
    /// <summary>
    /// Returns a point on the path at a specified percentage of the total path length.
    /// If the input percentage exceeds 1, it returns the last point on the path or loops to the start of the path based on the 'loop' parameter.
    /// </summary>
    /// <param name="percent">The percentage of the total path length at which to get the point (0 to 1).</param>
    /// <param name="loop">Optional parameter which if set to true, loops to the start of the path if the percentage is greater than 1. Defaults to false.</param>
    /// <param name="space">Optional parameter to specify if the coordinates should be in local or world space. Defaults to world space.</param>
    /// <returns>A Vector2 point at the specified percentage along the path.</returns>
    public Vector2 GetPositionByPercent(float percent, bool loop = false, Space space = Space.World)
    {
      if (_shouldInitialize)
      {
        _shouldInitialize = false;
        Initialize();
      }

      if (percent > 1)
      {
        percent -= (int)percent;
      }

      var dist = Length * percent;

      return GetPositionByDistance(dist, loop, space);
    }
    
    /// <summary>
    /// Converts a distance along the path to a percentage of the path's length.
    /// </summary>
    /// <param name="percent"></param>
    /// <returns>the distance</returns>
    public float PercentToDistance(float percent)
    {
      return Length * percent;
    }
    
    /// <summary>
    /// Converts a distance along the path to a percentage of the path's length.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public float DistanceToPercent(float distance)
    {
      return distance / Length;
    }
    
    /// <summary>
    /// Adds a new segment to the path at the specified anchor position.
    /// </summary>
    /// <param name="anchorPos">The position of the anchor point of the new segment in the space of the game object to
    /// which this component is attached.</param>
    public void AddSegment(Vector2 anchorPos)
    {
      _path.AddSegment(anchorPos - (Vector2)transform.position, _defaultControlType);
      Initialize();
    }
    
    /// <summary>
    /// Split a segment to the path at the specified anchor position.
    /// </summary>
    /// <param name="anchorPos">The position of the anchor point of the new segment in the space of the game object to
    /// which this component is attached.</param>
    /// <param name="segmentIndex">The index of the segment to split.</param>
    public void SplitSegment(Vector2 anchorPos, int segmentIndex)
    {
      _path.SplitSegment(anchorPos - (Vector2)transform.position, segmentIndex, _defaultControlType);
      Initialize();
    }
    
    /// <summary>
    /// Returns the node positions of the segment at the specified index.
    /// The returned positions are in the space of the game object to which this component is attached.
    /// </summary>
    /// <param name="segmentIndex"></param>
    /// <returns></returns>
    public Vector2[] GetNodesInSegment(int segmentIndex)
    {
      var nodes = _path.GetNodesInSegment(segmentIndex);
      return nodes?.Select(p => p + (Vector2)transform.position).ToArray();
    }
    
    /// <summary>
    /// Gets the position of the node at the specified index.
    /// </summary>
    /// <param name="pointIndex">The index of the node in the path.</param>
    /// <param name="space">(Optional) The space in which to return the position. Defaults to world space.</param>
    /// <returns>Returns the position of the node at the given index. If 'space' is set to 'Space.World',
    /// the position is relative to the world, otherwise it's relative to the transform of this GameObject.</returns>
    public Vector2 GetNodePosition(int pointIndex, Space space = Space.World)
    {
      var pos = _path[pointIndex];
      if (space == Space.World)
        pos += (Vector2)transform.position;
      return pos;
    }
    
    /// <summary>
    /// Moves the point at the specified index to a new position.
    /// </summary>
    /// <param name="pointIndex">The index of the point in the path to be moved.</param>
    /// <param name="position">The new position of the point.</param>
    /// <param name="force">(Optional) If true, the point will be moved regardless of any constraints that might exist. Defaults to false.</param>
    public void MovePoint(int pointIndex, Vector2 position, bool force = false)
    {
      _path.MoveNode(pointIndex, position - (Vector2)transform.position, force);
      Initialize();
    }
    
    /// <summary>
    /// Moves the entire segment by applying a movement vector.
    /// </summary>
    /// <param name="segmentIndex">The index of the segment in the path to be moved.</param>
    /// <param name="movement">The movement vector to be applied to the segment.</param>
    /// <param name="origin">The origin of the movement vector.</param>
    public void MoveSegment(int segmentIndex, Vector2 movement, Vector2 origin)
    {
      _path.MoveSegment(segmentIndex, movement, origin - (Vector2)transform.position);
      Initialize();
    }
    
    /// <summary>
    /// Calculates the positions of control points around an anchor for preview purposes.
    /// </summary>
    /// <param name="previewPos">The position where the preview of the control points should be calculated.</param>
    /// <param name="segmentIndex">The index of the segment where the anchor is located.</param>
    /// <returns>An array of control points positions.</returns>
    public Vector2[] CalculateAnchorControlPointsPositions(Vector2 previewPos, int segmentIndex)
    {
      return _path.CalculateControlPointsPositions(previewPos - (Vector2)transform.position, segmentIndex);
    }
    
    /// <summary>
    /// Returns all control points in the path.
    /// </summary>
    /// <param name="space">(Optional) The space in which the control points should be returned. Can be either World or Local. Defaults to World.</param>
    /// <returns>An array of control points in the specified space.</returns>
    public Vector2[] GetControlPoints(Space space = Space.World)
    {
      var controlPoints = new List<Vector2>();
      for (var i = 0; i < _path.NodeCount; i++)
      {
        var pos = GetNodePosition(i, space);
        controlPoints.Add(pos);
      }

      return controlPoints.ToArray();
    }
    
    /// <summary>
    /// Returns all the control points that are also anchor points in the path.
    /// </summary>
    /// <param name="space">(Optional) The space in which the control points should be returned.
    /// Can be either World or Local. Defaults to World.</param>
    /// <returns>An array of control points that are also anchor points in the specified space.</returns>
    public Vector2[] GetAnchorControlPoints(Space space = Space.World)
    {
      var controlPoints = new List<Vector2>();
      for (var i = 0; i < _path.NodeCount; i += 3)
      {
        var pos = GetNodePosition(i, space);
        controlPoints.Add(pos);
      }

      return controlPoints.ToArray();
    }
    
    /// <summary>
    /// Changes the type of the specified anchor point and optionally sets up control points.
    /// </summary>
    /// <param name="index">The index of the anchor point to change the type of.</param>
    /// <param name="type">The new NodeType for the anchor point.</param>
    /// <param name="setUp">(Optional) Indicates whether control points should be set up. Defaults to false.</param>
    public void ChangeAnchorType(int index, NodeType type, bool setUp = false)
    {
      _path.SetAnchorStatus(index * 3, type);
      Initialize();
    }

    /// <summary>
    /// Gets the NodeType of the specified anchor point.
    /// </summary>
    /// <param name="index">The index of the anchor point to get the type of.</param>
    /// <returns>The NodeType of the specified anchor point.</returns>
    public NodeType GetAnchorType(int index)
    {
      return _path.GetAnchorType(index * 3);
    }
    
    /// <summary>
    /// Finds the closest point on the spline to the given point.
    /// </summary>
    /// <param name="point">The point to find the closest spline point to.</param>
    /// <returns>The closest point on the spline as a Vector3.</returns>
    public Vector3 GetClosestSplinePoint(Vector2 point)
    {
      var min = 0;
      if (_nodes.Length == 0)
        Initialize();

      var minDist = Vector2.Distance(_nodes[0] + (Vector2)transform.position, point);

      for (var i = 1; i < _nodes.Length; i++)
      {
        var distance = Vector2.Distance(_nodes[i] + (Vector2)transform.position, point);

        if (!(distance < minDist)) continue;
        minDist = distance;
        min = i;
      }

      return (Vector3)_nodes[min] + transform.position;
    }
  }
}