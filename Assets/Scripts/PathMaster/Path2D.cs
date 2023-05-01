using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteNinja.PathMaster
{
  [Serializable]
  public class Path2D
  {
    /// <summary>
    /// List of nodes that make up the path.
    /// </summary>
    [SerializeField, HideInInspector] public List<Vector2> nodes;

    /// <summary>
    /// Indicates if the path is closed.
    /// </summary>
    [SerializeField, HideInInspector] private bool closed;

    /// <summary>
    /// Indicates if control points are set automatically.
    /// </summary>
    [SerializeField, HideInInspector] private bool setControlPointsAutomatically;

    /// <summary>
    /// The type of each anchor in the path.
    /// </summary>
    [SerializeField, HideInInspector] private List<NodeType> anchorTypes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Path2D"/> class at a specified center position with a specified node type.
    /// </summary>
    /// <param name="centre">The center position for initializing the path.</param>
    /// <param name="type">The default type of the nodes.</param>
    public Path2D(Vector2 centre, NodeType type)
    {
      if (type == NodeType.ANGULAR)
      {
        nodes = new List<Vector2>
        {
          centre + new Vector2(-1, 0),
          centre + new Vector2(-1, 0),
          centre + new Vector2(1, 0),
          centre + new Vector2(1, 0)
        };
      }
      else
      {
        nodes = new List<Vector2>
        {
          centre + new Vector2(-1, 0),
          centre + new Vector2(-1, 1) * 0.5f,
          centre + new Vector2(-1, -1) * 0.5f,
          centre + new Vector2(1, 0)
        };
      }

      anchorTypes = new List<NodeType> { type, type };
    }

    /// <summary>
    /// Indexer to get or set nodes using array index notation.
    /// </summary>
    /// <param name="i">The index of the node.</param>
    /// <returns>The node at the specified index.</returns>
    public Vector2 this[int i] => nodes[i];

    /// <summary>
    /// Gets or sets a value indicating whether the path is closed.
    /// </summary>
    public bool IsClosed
    {
      get => closed;
      set
      {
        if (closed == value) return;
        closed = value;

        if (closed)
        {
          nodes.Add(nodes[^1] * 2 - nodes[^2]);
          nodes.Add(nodes[0] * 2 - nodes[1]);
          if (setControlPointsAutomatically)
          {
            SetControlPoints(0);
            SetControlPoints(nodes.Count - 3);
          }

          UpdateControlPointsAroundAnchor(0);
          UpdateControlPointsAroundAnchor(nodes.Count - 3);
        }
        else
        {
          nodes.RemoveRange(nodes.Count - 2, 2);
          if (setControlPointsAutomatically)
          {
            SetAnchors();
          }
        }
      }
    }

    /// <summary>
    /// Gets the number of nodes in the path.
    /// </summary>
    public int NodeCount => nodes.Count;

    /// <summary>
    /// Number of segments in the path.
    /// </summary>
    public int SegmentCount => nodes.Count / 3;


    /// <summary>
    /// Adds a segment to the path at a specified position.
    /// </summary>
    /// <param name="anchorPos">The position of the anchor point of the new segment.</param>
    /// <param name="type">The type of the node being added.</param>
    public void AddSegment(Vector2 anchorPos, NodeType type)
    {
      var lastAnchor = nodes.Count - 1;
      var lastControl = nodes.Count - 2;

      if (GetAnchorType(lastAnchor) == NodeType.ANGULAR)
      {
        nodes.Add(nodes[lastAnchor]);
      }
      else
      {
        nodes.Add(nodes[lastAnchor] * 2 - nodes[lastControl]);
      }

      if (type == NodeType.ANGULAR)
      {
        nodes.Add(anchorPos);
      }
      else
      {
        nodes.Add((nodes[lastAnchor] + anchorPos) * 0.5f);
      }

      nodes.Add(anchorPos);

      if (GetAnchorType(lastAnchor) == NodeType.ANGULAR && type != NodeType.ANGULAR)
      {
        SetControlPoints(nodes.Count - 1);
      }

      if (setControlPointsAutomatically)
      {
        SetAllAffectedControlPoints(nodes.Count - 1);
      }

      anchorTypes.Add(type);
      UpdateControlPointsAroundAnchor(nodes.Count - 1);
    }

    /// <summary>
    /// Splits a segment at a specified position.
    /// </summary>
    /// <param name="anchorPos">The position where to split the segment.</param>
    /// <param name="segmentIndex">The index of the segment to be split.</param>
    /// <param name="type">The type of the node at the split position.</param>
    public void SplitSegment(Vector2 anchorPos, int segmentIndex, NodeType type)
    {
      nodes.InsertRange(segmentIndex * 3 + 2,
        type == NodeType.ANGULAR
          ? new[] { anchorPos, anchorPos, anchorPos }
          : new[] { Vector2.zero, anchorPos, Vector2.zero });

      anchorTypes.Insert(segmentIndex + 1, type);

      if (setControlPointsAutomatically)
      {
        SetAllAffectedControlPoints(segmentIndex * 3 + 3);
      }
      else
      {
        if (type != NodeType.ANGULAR)
          SetControlPoints(segmentIndex * 3 + 3);
      }

      UpdateControlPointsAroundAnchor(segmentIndex * 3 + 3);
    }

    /// <summary>
    /// Removes a segment from the path.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor of the segment to be removed.</param>
    public void RemoveSegment(int anchorIndex)
    {
      if (SegmentCount <= 2 && (closed || SegmentCount <= 1)) return;
      if (anchorIndex == 0)
      {
        if (closed)
        {
          nodes[^1] = nodes[2];
        }

        nodes.RemoveRange(0, 3);
      }
      else if (anchorIndex == nodes.Count - 1 && !closed)
      {
        nodes.RemoveRange(anchorIndex - 2, 3);
      }
      else
      {
        nodes.RemoveRange(anchorIndex - 1, 3);
      }

      anchorTypes.RemoveAt(anchorIndex / 3);
      UpdateControlPointsAroundAnchor(anchorIndex);
    }

    /// <summary>
    /// Gets the nodes in a specific segment.
    /// </summary>
    /// <param name="index">The index of the segment.</param>
    /// <returns>An array of nodes in the segment.</returns>
    public Vector2[] GetNodesInSegment(int index)
    {
      return index < SegmentCount
        ? new[] { nodes[index * 3], nodes[index * 3 + 1], nodes[index * 3 + 2], nodes[CircularIndex(index * 3 + 3)] }
        : null;
    }

    /// <summary>
    /// Gets the type of a specific anchor.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor.</param>
    /// <returns>The type of the anchor.</returns>
    public NodeType GetAnchorType(int anchorIndex)
    {
      return anchorTypes[anchorIndex / 3];
    }

    /// <summary>
    /// Sets the type of a specific anchor.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor.</param>
    /// <param name="type">The type to set for the anchor.</param>
    /// <param name="force">Whether to force the setting of the anchor type, regardless of current type.</param>
    public void SetAnchorStatus(int anchorIndex, NodeType type, bool force = false)
    {
      if (!force)
      {
        switch (type)
        {
          case NodeType.SMOOTH:
            SetControlPoints(anchorIndex);
            break;
          case NodeType.ANGULAR:
            var controlA = (NodeCount + anchorIndex - 1) % NodeCount;
            var controlB = (anchorIndex + 1) % NodeCount;

            if (anchorIndex > 0 || closed)
            {
              nodes[controlA] = nodes[anchorIndex];
            }

            if (anchorIndex < NodeCount - 1 || closed)
            {
              nodes[controlB] = nodes[anchorIndex];
            }

            break;
          case NodeType.FREE:
            if (GetAnchorType(anchorIndex) == NodeType.ANGULAR)
            {
              SetControlPoints(anchorIndex);
            }

            break;
          case NodeType.TANGENT:
            if (GetAnchorType(anchorIndex) == NodeType.ANGULAR || GetAnchorType(anchorIndex) == NodeType.FREE)
            {
              SetControlPoints(anchorIndex);
            }

            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
      }

      UpdateControlPointsAroundAnchor(anchorIndex);
      anchorTypes[anchorIndex / 3] = type;
    }

    /// <summary>
    /// Moves a node to a new position.
    /// </summary>
    /// <param name="i">The index of the node to move.</param>
    /// <param name="pos">The new position for the node.</param>
    /// <param name="force">Whether to force the node move, regardless of control point setting.</param>
    public void MoveNode(int i, Vector2 pos, bool force = false)
    {
      var deltaMove = pos - nodes[i];

      if (force)
      {
        nodes[i] = pos;
      }
      else
      {
        if (i % 3 != 0 && setControlPointsAutomatically) return;
        nodes[i] = pos;

        if (setControlPointsAutomatically)
        {
          SetAllAffectedControlPoints(i);
        }
        else
        {
          if (i % 3 == 0)
          {
            if (i + 1 < nodes.Count || closed)
            {
              nodes[CircularIndex(i + 1)] += deltaMove;
            }

            if (i - 1 >= 0 || closed)
            {
              nodes[CircularIndex(i - 1)] += deltaMove;
            }

            if (GetAnchorType(i) == NodeType.SMOOTH)
            {
              SetControlPoints(i);
            }

            UpdateControlPointsAroundAnchor(i);
          }
          else
          {
            int correspondingControlIndex, anchorIndex;

            if ((i + 1) % 3 == 0)
            {
              if (i + 1 < nodes.Count || !closed)
              {
                correspondingControlIndex = i + 2;
                anchorIndex = i + 1;
              }
              else
              {
                correspondingControlIndex = 1;
                anchorIndex = 0;
              }
            }
            else
            {
              anchorIndex = i - 1;
              if (i > 1 || !closed)
              {
                correspondingControlIndex = i - 2;
              }
              else
              {
                correspondingControlIndex = nodes.Count - 1;
              }
            }

            if (GetAnchorType(anchorIndex) != NodeType.TANGENT) return;
            if (correspondingControlIndex < 0 || correspondingControlIndex >= nodes.Count) return;

            var dist = (nodes[CircularIndex(anchorIndex)] - nodes[CircularIndex(correspondingControlIndex)]).magnitude;
            var dir = (nodes[CircularIndex(anchorIndex)] - pos).normalized;
            nodes[CircularIndex(correspondingControlIndex)] = nodes[CircularIndex(anchorIndex)] + dir * dist;
          }
        }
      }
    }

    /// <summary>
    /// Updates the control points around a specific anchor.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor.</param>
    public void UpdateControlPointsAroundAnchor(int anchorIndex)
    {
      for (var j = anchorIndex - 3; j >= 0; j -= 3)
      {
        if (GetAnchorType(j) == NodeType.SMOOTH)
        {
          SetControlPoints(j);
        }
        else
        {
          break;
        }
      }

      for (var j = anchorIndex + 3; j < NodeCount; j += 3)
      {
        if (GetAnchorType(j) == NodeType.SMOOTH)
        {
          SetControlPoints(j);
        }
        else
        {
          break;
        }
      }
    }

    /// <summary>
    /// Generates a set of equidistant nodes along the path.
    /// </summary>
    /// <param name="spacing">The distance between generated nodes.</param>
    /// <returns>An array of equidistant nodes.</returns>
    public Vector2[] GetEquidistantNodes(float spacing)
    {
      var equidistantNodes = new List<Vector2> { nodes[0] };
      var lastPoint = nodes[0];
      float distSinceLastEDPoint = 0;

      for (var segmentI = 0; segmentI < SegmentCount; segmentI++)
      {
        var p = GetNodesInSegment(segmentI);
        var controlNetLenght =
          Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
        var estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLenght / 2f;
        var divisions = Mathf.CeilToInt(estimatedCurveLength * 10);
        float t = 0;
        while (t <= 1)
        {
          t += 1f / divisions;
          var pointOnCurve = BezierHelper.EvaluateCubic(p[0], p[1], p[2], p[3], t);
          distSinceLastEDPoint += Vector2.Distance(lastPoint, pointOnCurve);

          while (distSinceLastEDPoint >= spacing)
          {
            var overshootDist = distSinceLastEDPoint - spacing;
            var newEvenlySpacedPoint = pointOnCurve + (lastPoint - pointOnCurve).normalized * overshootDist;
            equidistantNodes.Add(newEvenlySpacedPoint);
            distSinceLastEDPoint = overshootDist;
            lastPoint = newEvenlySpacedPoint;
          }

          lastPoint = pointOnCurve;
        }
      }

      return equidistantNodes.ToArray();
    }

    /// <summary>
    /// Sets the control points around a specific anchor.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor.</param>
    public void SetControlPoints(int anchorIndex)
    {
      var anchorPos = nodes[anchorIndex];
      var dir = new Vector2();
      var neighbourDistances = new float[2];

      if (anchorIndex - 3 >= 0 || closed)
      {
        var offset = nodes[CircularIndex(anchorIndex - 3)] - anchorPos;
        dir += offset.normalized;
        neighbourDistances[0] = offset.magnitude;
      }

      if (anchorIndex + 3 >= 0 || closed)
      {
        var offset = nodes[CircularIndex(anchorIndex + 3)] - anchorPos;
        dir -= offset.normalized;
        neighbourDistances[1] = -offset.magnitude;
      }

      dir.Normalize();

      for (var i = 0; i < 2; i++)
      {
        var controlIndex = anchorIndex + i * 2 - 1;
        if (controlIndex >= 0 && controlIndex < nodes.Count || closed)
        {
          nodes[CircularIndex(controlIndex)] = anchorPos + dir * (neighbourDistances[i] * 0.5f);
        }
      }
    }

    /// <summary>
    /// Calculates the positions of control points for a specific anchor in the path, based on the position and segment index.
    /// </summary>
    /// <param name="position">The position of the anchor.</param>
    /// <param name="segment">The index of the segment in which the anchor is located.</param>
    /// <returns>An array of Vector2 containing the calculated control points' positions.</returns>
    public Vector2[] CalculateControlPointsPositions(Vector2 position, int segment)
    {
      var dir = new Vector2();
      var neighbourDistances = new float[2];

      if (segment * 3 - 3 >= 0 || closed)
      {
        var offset = nodes[CircularIndex(segment * 3 - 3)] - position;
        dir += offset.normalized;
        neighbourDistances[0] = offset.magnitude;
      }

      if (segment * 3 + 3 >= 0 || closed)
      {
        var offset = nodes[CircularIndex(segment * 3 + 3)] - position;
        dir -= offset.normalized;
        neighbourDistances[1] = -offset.magnitude;
      }

      dir.Normalize();


      var ret = new List<Vector2>();
      for (var i = 0; i < 2; i++)
      {
        ret.Add(position + dir * (neighbourDistances[i] * 0.5f));
      }

      return ret.ToArray();
    }

    /// <summary>
    /// Translates the path by a given vector.
    /// </summary>
    /// <param name="translation"></param>
    public void Translate(Vector3 translation)
    {
      for (var i = 0; i < nodes.Count; i++)
      {
        nodes[i] += (Vector2)translation;
      }
    }

    /// <summary>
    /// Moves a segment of the path by a certain amount from the given origin. 
    /// The method adjusts the type of the anchor points to ensure smooth movement and adjusts the positions of the
    /// anchor's control points proportionally.
    /// </summary>
    /// <param name="segment">The index of the segment to be moved.</param>
    /// <param name="movement">The vector representing the direction and magnitude of the movement.</param>
    /// <param name="origin">The original position from which the movement starts.</param>
    public void MoveSegment(int segment, Vector2 movement, Vector2 origin)
    {
      var distToA = (nodes[segment * 3 + 1] - origin).magnitude;
      var distToB = (nodes[segment * 3 + 2] - origin).magnitude;

      if (GetAnchorType(segment * 3) == NodeType.ANGULAR)
        SetAnchorStatus(segment * 3, NodeType.FREE, true);

      if (GetAnchorType(segment * 3) == NodeType.SMOOTH)
        SetAnchorStatus(segment * 3, NodeType.TANGENT);

      if (GetAnchorType((segment * 3 + 3) % nodes.Count) == NodeType.ANGULAR)
        SetAnchorStatus((segment * 3 + 3) % nodes.Count, NodeType.FREE, true);

      if (GetAnchorType((segment * 3 + 3) % nodes.Count) == NodeType.SMOOTH)
        SetAnchorStatus((segment * 3 + 3) % nodes.Count, NodeType.TANGENT);

      MoveNode(segment * 3 + 1, nodes[segment * 3 + 1] + movement * distToB / (distToA + distToB));
      MoveNode(segment * 3 + 2, nodes[segment * 3 + 2] += movement * distToA / (distToA + distToB));
    }

    /// <summary>
    /// Sets all affected control points for a given anchor index. 
    /// The method also calls the SetAnchors method to adjust the positions of the first and last control points in case the path is not closed.
    /// </summary>
    /// <param name="anchorIndex">The index of the anchor whose affected control points need to be set.</param>
    private void SetAllAffectedControlPoints(int anchorIndex)
    {
      for (var i = anchorIndex - 3; i < anchorIndex + 3; i += 3)
      {
        if (i >= 0 && i < nodes.Count || closed)
        {
          SetControlPoints(CircularIndex(i));
        }
      }

      SetAnchors();
    }

    /// <summary>
    /// Adjusts the positions of the first and last control points of the path.
    /// This method is called only if the path is not closed.
    /// </summary>
    private void SetAnchors()
    {
      if (closed) return;
      nodes[1] = (nodes[0] + nodes[2]) * 0.5f;
      nodes[^2] = (nodes[^1] + nodes[^3]) * 0.5f;
    }

    /// <summary>
    /// Returns the circular index for a given index in the nodes list.
    /// This method is used to handle the circular nature of a closed path.
    /// </summary>
    /// <param name="i">The index to convert.</param>
    /// <returns>The circular index corresponding to the input index.</returns>
    private int CircularIndex(int i)
    {
      return (i + nodes.Count) % nodes.Count;
    }
  }
}