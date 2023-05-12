using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LiteNinja.PathMaster.Editors
{
  [CustomEditor(typeof(MonoPath2D)), CanEditMultipleObjects]
  public class MonoPath2DEditor : Editor
  {
    private MonoPath2D _monoPath2D;
    private Path2D Path2D => _monoPath2D.Path;

#region Colors

    private readonly Color _nodeColor = Color.white;
    private readonly Color _selectedNodeColor = Color.green;
    private readonly Color _removeNodeColor = Color.red;
    private readonly Color _curveColor = Color.white;
    private readonly Color _selectedCurveColor = new Color(1f, 0.5f, 0f);
    private readonly Color _controlPointColor = Color.cyan;
    private readonly Color _controlLineColor = Color.cyan;
    private readonly Color _splitLineColor = Color.yellow;
    private readonly Color _selectionColor = new(0.7f, 1f, 0.5f, 0.3f);
    private readonly Color _selectionBorderColor = new(0.2f, 0.3f, 0f, 1f);

#endregion

#region Handles Sizes and Distances

    private const float _anchorDiameter = 0.17f;
    private const float _controlDiameter = 0.059f;
    private const float _addDiameter = 0.05f;
    private const float _anchorSelectOffset = 0.3f;
    private const float _controlSelectOffset = _controlDiameter + 0.02f;
    private const float _segmentSelectDistanceThreshold = 0.02f;

#endregion


    private int _selectedSegmentIndex = -1;
    private int _selectedNode = -1;
    private int _clickedNode = -1;
    private bool _isDragging;

    private bool _anchorsFoldout = true;

    private readonly List<int> _selectedNodes = new();
    private readonly List<int> _nodesSelectedInCurrentDrag = new();

    private bool _dragSelection;
    private bool _dragSegment;
    private bool _isCurveDraggable;
    private Vector2 _dragStartPosition;
    private Vector2 _dragCurrentPosition;

    private bool _deselect;
    private int _movingSegmentIndex = -1;
    private bool _isPressingCKey;

    private enum AnchorStatus
    {
      Normal,
      Remove,
      Selected,
    }

    private void OnEnable()
    {
      _monoPath2D = (MonoPath2D)target;
      if (_monoPath2D.Path == null)
      {
        _monoPath2D.CreatePath();
      }
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      EditorGUI.BeginChangeCheck();
      DrawCloseGUI();
      DrawSpacingGUI();
      DrawNodeTypeGUI();
      
      DrawAnchorList();
      
      DrawCenterButton();
      DrawSetOriginAsFirstButton();
      DrawSetOriginAsLastButton();
      
      DrawResetButton();

      if (EditorGUI.EndChangeCheck())
      {
        SceneView.RepaintAll();
      }

      ShowPathInfo();
      ShowKeyboardMouseCommands();
    }


    private void OnSceneGUI()
    {
      Input();
      Draw();
    }

#region OnInspectorGUI

    private void DrawCloseGUI()
    {
      var isClosed = EditorGUILayout.Toggle("Close Path", Path2D.IsClosed);
      if (isClosed == Path2D.IsClosed) return;
      Undo.RecordObject(_monoPath2D, "Toggle Closed");
      Path2D.IsClosed = isClosed;
    }

    private void DrawSpacingGUI()
    {
      var spacing = EditorGUILayout.FloatField("Spacing", _monoPath2D.Spacing);
      if (spacing < 0.01f) spacing = 0.01f;
      if (spacing == _monoPath2D.Spacing) return;
      Undo.RecordObject(_monoPath2D, "Set Spacing");
      _monoPath2D.Spacing = spacing;
    }


    private void DrawNodeTypeGUI()
    {
      GUILayout.Space(10);
      var defaultNodeType = EditorGUILayout.Popup("Default Anchor Type", (int)_monoPath2D.DefaultControlType,
        new[] { "Smooth", "Tangent", "Free", "Angular" });
      if (defaultNodeType != (int)_monoPath2D.DefaultControlType)
      {
        Undo.RecordObject(_monoPath2D, "Set Default Anchor Type");
        _monoPath2D.DefaultControlType = (NodeType)defaultNodeType;
      }

      if (_selectedNode == -1 && _selectedNodes.Count <= 0) return;

      GUILayout.Space(10);
      var areAllTheSame = true;
      NodeType sharedNodeType;
      if (_selectedNodes.Count > 0)
      {
        areAllTheSame = AreSelectedNodesTheSameType();
        sharedNodeType = Path2D.GetAnchorType(_selectedNodes[0]);
      }
      else
      {
        sharedNodeType = Path2D.GetAnchorType(_selectedNode);
      }

      var label = _selectedNodes.Count > 0 ? "Selected Nodes Type" : "Selected Anchor Type";
      var options = _selectedNodes.Count > 0 && !areAllTheSame
        ? new[] { "Smooth", "Tangent", "Free", "Angular", "-" }
        : new[] { "Smooth", "Tangent", "Free", "Angular" };
      var currentIndex = _selectedNodes.Count > 0 && !areAllTheSame ? 4 : (int)sharedNodeType;
      var nodeType = EditorGUILayout.Popup(label, currentIndex, options);

      if (nodeType == currentIndex || nodeType == 4) return;

      Undo.RecordObject(_monoPath2D, "Set Nodes Type");
      SetAnchorStatus(_selectedNode, (NodeType)nodeType, true);
    }

    private void DrawAnchorList()
    {
      var anchorIndexes = new List<int>();
      var controlPointIndexes = new List<int>();
      
      for (var i = 0; i < Path2D.NodeCount; i++)
      {
        if (i % 3 == 0)
        {
          anchorIndexes.Add(i);
        }
        else
        {
          controlPointIndexes.Add(i);
        }
      }

      _anchorsFoldout = EditorGUILayout.Foldout(_anchorsFoldout, "Anchors:");
      if (_anchorsFoldout)
      {
        var deleteIndexes = anchorIndexes.Where(DrawAnchorDrawer).ToList();
        if (deleteIndexes.Count > 0)
        {
          Undo.RecordObject(_monoPath2D, "Delete Anchors");
          foreach (var index in deleteIndexes)
          {
            Path2D.RemoveSegment(index);
          }
        }
        
      }
    }

    private bool DrawAnchorDrawer(int anchorIndex)
    {
      var position = Path2D[anchorIndex];
      var nodeType = Path2D.GetAnchorType(anchorIndex);
      EditorGUILayout.BeginHorizontal();
      var newPosition = EditorGUILayout.Vector2Field("", position);

      if (newPosition != position)
      {
        Undo.RecordObject(_monoPath2D, "Move Anchor");
        Path2D.MoveNode(anchorIndex, newPosition);
        SetAnchorStatus(anchorIndex, nodeType, true);
      }

      var newType = EditorGUILayout.Popup("", (int)nodeType,
        new[] { "Smooth", "Tangent", "Free", "Angular" }, GUILayout.Width(80));
      if (newType != (int)nodeType)
      {
        Undo.RecordObject(_monoPath2D, "Set Anchor Type");
        SetAnchorStatus(anchorIndex, (NodeType)newType, true);
      }

      bool remove = GUILayout.Button("X", GUILayout.Width(20));

      EditorGUILayout.EndHorizontal();
      return remove;
    }

    private void DrawCenterButton()
    {
      GUILayout.Space(20);
      if (!GUILayout.Button("Center")) return;
      if (Path2D.NodeCount <= 0) return;
      Undo.RecordObject(_monoPath2D, "Center");
      
      Vector2 center = _monoPath2D.gameObject.transform.position;
      //Calc the average position of all the anchors
      var currentCenter = Vector2.zero;
      var numAnchors = 0;
      for (var i = 0; i < Path2D.NodeCount; i++)
      {
        if (i % 3 != 0) continue;
        currentCenter += Path2D[i] + center;
        numAnchors++;
      }

      currentCenter /= numAnchors;
      // Calc the offset
      var offset = center - currentCenter;
      Path2D.Translate(offset);
    }

    private void DrawSetOriginAsFirstButton()
    {
      if (!GUILayout.Button("Set First Node in the Origin")) return;
      if (Path2D.NodeCount <= 0) return;
      Undo.RecordObject(_monoPath2D, "Set First Node in the Origin");
      
      Vector2 center = _monoPath2D.gameObject.transform.position;
      var firstNode = Path2D[0] + center;
      Path2D.Translate(-firstNode);
    }

    private void DrawSetOriginAsLastButton()
    {
      if (!GUILayout.Button("Set Last Node in the Origin")) return;
      if (Path2D.NodeCount <= 0) return;
      Undo.RecordObject(_monoPath2D, "Set Last Node in the Origin");
      
      Vector2 center = _monoPath2D.gameObject.transform.position;
      var lastNodeIndex = Path2D.NodeCount - 1;
      if (Path2D.IsClosed && Path2D.NodeCount > 2)
      {
        lastNodeIndex = Path2D.NodeCount - 2;
      }
      
      var lastNode = Path2D[lastNodeIndex] + center;
      Path2D.Translate(-lastNode);
    }

    private void DrawResetButton()
    {
      GUILayout.Space(20);
      if (!GUILayout.Button("Reset")) return;

      Undo.RecordObject(_monoPath2D, "Reset");
      _selectedSegmentIndex = -1;
      _monoPath2D.Reset();
      SceneView.RepaintAll();
    }

    private static void ShowKeyboardMouseCommands()
    {
      GUILayout.Space(10);
      GUILayout.Label("Keyboard / Mouse Commands", EditorStyles.boldLabel);
      GUILayout.Label("Shift + Left Mouse Click: Add a node (if the curve is not closed)");
      GUILayout.Label("Left Mouse Pressed: Drag Selected Nodes or Drag Nearest Anchor");
      GUILayout.Label("Left Mouse Click: Toggle Select Anchor");
      GUILayout.Label("Esc: Unselect Selected Nodes");
      GUILayout.Label("Shift + Ctrl + Left Mouse Click: Add a node to the nearest segment");
      GUILayout.Label("Hold C key: Toggle Curve Dragging");
      GUILayout.Label("Shift + Left Mouse Click + Drag: Select Nodes");
      GUILayout.Label("Hold Ctrl key: Toggle Anchor Deletion");
      GUILayout.Label("Ctrl key + Left Mouse Click: Delete Anchor");
    }

    private void ShowPathInfo()
    {
      // Display a text box with the path information
      GUILayout.Space(10);
      GUILayout.Label("Path Information", EditorStyles.boldLabel);
      GUILayout.Label($"Nodes: {Path2D.nodes.Count}");
      GUILayout.Label($"Length: {_monoPath2D.Length}");
    }

    private bool AreSelectedNodesTheSameType()
    {
      var stat = Path2D.GetAnchorType(_selectedNodes[0]);
      return _selectedNodes.All(i => Path2D.GetAnchorType(i) == stat);
    }

    private void SetAnchorStatus(int anchorIndex, NodeType type, bool setSelection = false)
    {
      Undo.RecordObject(_monoPath2D, "Change Anchor Type");
      if (setSelection && _selectedNodes.Count > 0)
      {
        foreach (var i in _selectedNodes)
        {
          Path2D.SetAnchorStatus(i, type);
        }
      }

      if (anchorIndex != -1)
      {
        Path2D.SetAnchorStatus(anchorIndex, type);
      }
    }

#endregion


#region OnSceneGUI

#region Input

    private void Input()
    {
      var currentEvent = Event.current;
      Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition).origin;

      DragCurveInput(currentEvent, mousePosition);
      DeleteKeyInput(currentEvent);
      DeselectInput(currentEvent);
      DrawNewNodeToTheNearestSegmentInput(currentEvent, mousePosition);
      SelectNodeInput(currentEvent, mousePosition);
      MouseDragInput(currentEvent, mousePosition);
      DragNodesInput(currentEvent, mousePosition);
      DeleteNodeInput(currentEvent, mousePosition);
      ClearLeftMouseUpInput(currentEvent);
      AddNewNodeToTheNearestSegmentInput(currentEvent, mousePosition);
      AddNewNodeInput(currentEvent, mousePosition);

      HandleUtility.AddDefaultControl(0);
    }

    private void AddNewNodeInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.type != EventType.MouseUp || currentEvent.button != 0 || !currentEvent.shift ||
          currentEvent.alt ||
          currentEvent.control) return;
      if (IsClickingNode(mousePosition) || _selectedNodes.Count != 0 ||
          Path2D.IsClosed) return;
      Undo.RecordObject(_monoPath2D, "Add segment");
      _monoPath2D.AddSegment(mousePosition);

      HandleUtility.Repaint();
    }

    private void AddNewNodeToTheNearestSegmentInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.type != EventType.MouseUp || currentEvent.button != 0 || !currentEvent.shift ||
          currentEvent.alt ||
          !currentEvent.control) return;
      if (_selectedSegmentIndex == -1 || IsClickingNode(mousePosition) ||
          _selectedNodes.Count != 0) return;
      Undo.RecordObject(_monoPath2D, "Split segment");
      _monoPath2D.SplitSegment(mousePosition, _selectedSegmentIndex);
      HandleUtility.Repaint();
    }

    private void ClearLeftMouseUpInput(Event currentEvent)
    {
      if (currentEvent.type != EventType.MouseUp || currentEvent.button != 0) return;
      _isDragging = false;
      _dragSelection = false;

      if (_deselect)
      {
        _selectedNodes.Clear();
      }
      else
      {
        foreach (var i in _nodesSelectedInCurrentDrag.Where(i => !_selectedNodes.Contains(i)))
        {
          _selectedNodes.Add(i);
        }

        _nodesSelectedInCurrentDrag.Clear();
      }

      _dragSegment = false;
      _movingSegmentIndex = -1;
      Repaint();
    }

    private void DeleteNodeInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.type != EventType.MouseDown || currentEvent.button != 0 || !currentEvent.control ||
          currentEvent.shift) return;
      var closest = ClosestNodeIndex(mousePosition);
      if (closest == -1) return;
      Undo.RecordObject(_monoPath2D, "Delete Anchor");
      Path2D.RemoveSegment(closest);
      _selectedSegmentIndex = -1;
    }

    private void DragNodesInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.shift || currentEvent.alt || currentEvent.control) return;
      if (currentEvent.button != 0) return;

      switch (currentEvent.type)
      {
        case EventType.MouseDown:
          OnDragNodeMouseDown(mousePosition);
          break;
        case EventType.MouseUp:
          OnDragNodeMouseUp();
          break;
      }
    }

    private void OnDragNodeMouseUp()
    {
      if (!_isDragging)
      {
        if (_clickedNode != -1)
        {
          if (_selectedNode != _clickedNode)
          {
            _selectedNode = _clickedNode;
          }
          else
          {
            _selectedNode = -1;
          }
        }
        else
        {
          if (_clickedNode == -1)
          {
            _selectedNode = -1;
          }
        }
      }

      HandleUtility.Repaint();
    }

    private void OnDragNodeMouseDown(Vector2 mousePosition)
    {
      var nearestNodeIndex = ClosestNodeIndex(mousePosition);

      if (nearestNodeIndex == -1)
      {
        if (!IsClickingNode(mousePosition))
        {
          if (_selectedSegmentIndex != -1 && _isCurveDraggable)
          {
            _dragSegment = true;
            _dragStartPosition = mousePosition;
            _dragCurrentPosition = mousePosition;
            _movingSegmentIndex = _selectedSegmentIndex;
          }

          _deselect = true;
        }

        _clickedNode = -1;
      }
      else if (nearestNodeIndex != _clickedNode)
      {
        _clickedNode = nearestNodeIndex;
        _dragSelection = false;
      }
    }

    private void MouseDragInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.type != EventType.MouseDrag) return;
      _isDragging = true;
      if (_dragSelection)
      {
        _selectedSegmentIndex = -1;
        _dragCurrentPosition = mousePosition;
        UpdateSelection();
        HandleUtility.Repaint();
      }

      if (_dragSegment)
      {
        Undo.RecordObject(_monoPath2D, "Move Curve");
        _monoPath2D.MoveSegment(_movingSegmentIndex, mousePosition - _dragCurrentPosition, _dragStartPosition);
        _dragCurrentPosition = mousePosition;
        HandleUtility.Repaint();
      }

      _deselect = false;
    }

    private void SelectNodeInput(Event currentEvent, Vector2 mousePosition)
    {
      if (currentEvent.type != EventType.MouseDown || currentEvent.button != 0 || !currentEvent.shift ||
          currentEvent.alt ||
          currentEvent.control) return;
      var closest = ClosestNodeIndex(mousePosition);
      if (closest == -1)
      {
        _dragSelection = true;
        _dragStartPosition = mousePosition;
        _dragCurrentPosition = mousePosition;
      }
      else
      {
        
        if (!_selectedNodes.Contains(closest))
        {
          _selectedNodes.Add(closest);
          _deselect = false;
          if (_selectedNode == -1) return;
          _selectedNodes.Add(_selectedNode);
          _selectedNode = -1;
        }
        else
        {
          _selectedNodes.Remove(closest);
        }
      }
    }

    private void DrawNewNodeToTheNearestSegmentInput(Event currentEvent, Vector2 mousePosition)
    {
      if (!currentEvent.shift || !currentEvent.control) return;
      if (_selectedSegmentIndex == -1) return;
      var segmentPoints = _monoPath2D.GetNodesInSegment(_selectedSegmentIndex);

      if (_monoPath2D.DefaultControlType == NodeType.ANGULAR)
      {
        Handles.DrawBezier(segmentPoints[0], mousePosition,
          segmentPoints[1], mousePosition, Color.grey, null, 2);
        Handles.DrawBezier(segmentPoints[3], mousePosition,
          segmentPoints[2], mousePosition, Color.grey, null, 2);
      }
      else
      {
        var controlAux = Path2D.CalculateControlPointsPositions(mousePosition, _selectedSegmentIndex);
        Handles.DrawBezier(segmentPoints[0], mousePosition,
          segmentPoints[1], controlAux[0], Color.grey, null, 2);
        Handles.DrawBezier(segmentPoints[3], mousePosition,
          segmentPoints[2], controlAux[1], Color.grey, null, 2);
      }

      HandleUtility.Repaint();
    }

    private void DeselectInput(Event currentEvent)
    {
      if ((currentEvent.type != EventType.KeyDown || currentEvent.keyCode != KeyCode.Escape) &&
          (currentEvent.type != EventType.MouseUp || currentEvent.button != 1)) return;
      _deselect = true;
      _selectedNode = -1;
      _selectedNodes.Clear();
      HandleUtility.Repaint();
    }

    private void DeleteKeyInput(Event currentEvent)
    {
      if (currentEvent.type != EventType.KeyDown || currentEvent.keyCode != KeyCode.Delete) return;
      GUIUtility.hotControl = 0;
      Event.current.Use();
      if (_isDragging || _dragSelection) return;
      _selectedNodes.Sort();
      for (var i = _selectedNodes.Count - 1; i >= 0; i--)
      {
        _selectedSegmentIndex = -1;
        Undo.RecordObject(_monoPath2D, "Delete Anchor");
        Path2D.RemoveSegment(_selectedNodes[i]);
      }

      if (_selectedNode != -1 && !_selectedNodes.Contains(_selectedNode))
      {
        Path2D.RemoveSegment(_selectedNode);
        _selectedNode = -1;
      }

      _selectedNodes.Clear();

      HandleUtility.Repaint();
    }

    private void DragCurveInput(Event currentEvent, Vector2 mousePosition)
    {
      switch (currentEvent.type)
      {
        case EventType.KeyDown when currentEvent.keyCode == KeyCode.C:
          GUIUtility.hotControl = 0;
          Event.current.Use();
          _isPressingCKey = true;
          break;
        case EventType.KeyUp when currentEvent.keyCode == KeyCode.C:
          _isPressingCKey = false;
          break;
        case EventType.MouseMove:
        {
          float smallestDistance = 100000;
          var nearestSegmentIndex = -1;

          for (var i = 0; i < Path2D.SegmentCount; i++)
          {
            var points = _monoPath2D.GetNodesInSegment(i);
            var dist = HandleUtility.DistancePointBezier(mousePosition, points[0],
              points[3],
              points[1], points[2]);

            if (!(dist < smallestDistance)) continue;
            smallestDistance = dist;
            nearestSegmentIndex = i;
          }

          if (smallestDistance < _segmentSelectDistanceThreshold && _isPressingCKey)
          {
            _isCurveDraggable = true;
          }
          else
          {
            _isCurveDraggable = false;
          }

          HandleUtility.Repaint();
          if (nearestSegmentIndex != _selectedSegmentIndex)
          {
            _selectedSegmentIndex = nearestSegmentIndex;
            HandleUtility.Repaint();
          }

          break;
        }
      }
    }

    private int ClosestNodeIndex(Vector2 mousePosition)
    {
      var smallestDistance = _anchorSelectOffset;
      var closestAnchorIndex = -1;

      for (var i = 0; i < Path2D.NodeCount; i += 3)
      {
        var dist = Vector2.Distance(mousePosition, _monoPath2D.GetNodePosition(i));
        if (!(dist < smallestDistance)) continue;
        smallestDistance = dist;
        closestAnchorIndex = i;
      }

      return closestAnchorIndex;
    }


    private bool IsClickingNode(Vector2 mousePosition)
    {
      for (var i = 0; i < Path2D.NodeCount; i++)
      {
        var minDist = i % 3 == 0 ? _anchorSelectOffset * 0.5f : _controlSelectOffset * 0.5f;

        var dist = Vector2.Distance(mousePosition, _monoPath2D.GetNodePosition(i));
        if (dist < minDist)
        {
          return true;
        }
      }

      return false;
    }

    private void UpdateSelection()
    {
      var selectedIndexes = new List<int>();

      for (var i = 0; i < Path2D.NodeCount; i += 3)
      {
        if (Mathf.Abs(_dragStartPosition.x - _monoPath2D.GetNodePosition(i).x) <=
            Mathf.Abs(_dragStartPosition.x - _dragCurrentPosition.x) &&
            Mathf.Abs(_dragCurrentPosition.x - _monoPath2D.GetNodePosition(i).x) <=
            Mathf.Abs(_dragStartPosition.x - _dragCurrentPosition.x) &&
            Mathf.Abs(_dragStartPosition.y - _monoPath2D.GetNodePosition(i).y) <=
            Mathf.Abs(_dragStartPosition.y - _dragCurrentPosition.y) &&
            Mathf.Abs(_dragCurrentPosition.y - _monoPath2D.GetNodePosition(i).y) <=
            Mathf.Abs(_dragStartPosition.y - _dragCurrentPosition.y))
        {
          if (!_selectedNodes.Contains(i) || !_nodesSelectedInCurrentDrag.Contains(i)) selectedIndexes.Add(i);
        }
        else
        {
          if (_nodesSelectedInCurrentDrag.Contains(i)) _nodesSelectedInCurrentDrag.Remove(i);
          if (selectedIndexes.Contains(i)) selectedIndexes.Remove(i);
        }
      }

      foreach (var i in selectedIndexes.Where(i => !_nodesSelectedInCurrentDrag.Contains(i)))
      {
        _nodesSelectedInCurrentDrag.Add(i);
      }
    }

#endregion

#region Draw

    private void Draw()
    {
      var anchorIndexes = new List<int>();
      var controlPointIndexes = new List<int>();

      for (var i = 0; i < Path2D.NodeCount; i++)
      {
        if (i % 3 == 0)
        {
          anchorIndexes.Add(i);
        }
        else
        {
          controlPointIndexes.Add(i);
        }
      }

      DrawSegments();
      DrawAnchors(anchorIndexes);
      DrawSplitButton();
      DrawControlPoints(controlPointIndexes);
      DrawSelectionArea();
    }

    private void DrawSelectionArea()
    {
      if (!_dragSelection) return;
      Handles.color = _selectionColor;
      Handles.DrawSolidRectangleWithOutline(new Rect(_dragStartPosition, _dragCurrentPosition - _dragStartPosition),
        _selectionColor,
        _selectionBorderColor);
    }

    private void DrawControlPoints(List<int> controlPointIndexes)
    {
      foreach (var i in controlPointIndexes)
      {
        var hideControlPoint = false;
        var j = 0;
        if ((i + 1) % 3 != 0)
        {
          j = (Path2D.NodeCount + i - 1) % Path2D.NodeCount;
        }
        else
        {
          j = (i + 1) % Path2D.NodeCount;
        }

        if (_monoPath2D.GetNodePosition(j) == _monoPath2D.GetNodePosition(i) ||
            Path2D.GetAnchorType(j) == NodeType.SMOOTH)
          hideControlPoint = true;

        if (hideControlPoint) continue;
        
        Handles.color = _controlPointColor;
        Vector2 newPos = Handles.FreeMoveHandle(_monoPath2D.GetNodePosition(i), Quaternion.identity, _controlDiameter,
          new Vector2(),
          Handles.CircleHandleCap);
        
        if (_monoPath2D.GetNodePosition(i) == newPos) continue;
        
        Undo.RecordObject(_monoPath2D, "Move Control Point");
        _monoPath2D.MovePoint(i, newPos);
      }
    }

    private void DrawSplitButton()
    {
      if (_selectedSegmentIndex == -1) return;
      
      var p = _monoPath2D.GetNodesInSegment(_selectedSegmentIndex);
      if (p == null) return;
      
      var midPoint = BezierHelper.EvaluateCubic(p[0], p[1], p[2], p[3], 0.5f);
      Handles.color = _splitLineColor;
      
      Handles.DrawLine(midPoint + Vector2.up * (_addDiameter * 0.4f),
        midPoint + Vector2.down * (_addDiameter * 0.4f));
      Handles.DrawLine(midPoint + Vector2.left * (_addDiameter * 0.4f),
        midPoint + Vector2.right * (_addDiameter * 0.4f));
      if (Handles.Button(midPoint, Quaternion.identity, _addDiameter, _addDiameter, Handles.CircleHandleCap))
      {
        Undo.RecordObject(_monoPath2D, "Split segment");
        _monoPath2D.SplitSegment(midPoint, _selectedSegmentIndex);
      }
    }

    private void DrawAnchors(List<int> anchorIndexes)
    {
      Handles.color = _nodeColor;
      
      foreach (var i in anchorIndexes)
      {
        var anchorType = AnchorStatus.Normal;
        if (_selectedNodes.Contains(i) || _nodesSelectedInCurrentDrag.Contains(i))
        {
          anchorType = AnchorStatus.Selected;
        }
        else if (Event.current.control && !Event.current.shift)
        {
          anchorType = AnchorStatus.Remove;
        }

        if (_selectedNode == i)
        {
          anchorType = AnchorStatus.Selected;
        }

        Vector2 newPos = Handles.FreeMoveHandle(_monoPath2D.GetNodePosition(i), Quaternion.identity,
          _anchorDiameter, new Vector2(), Handles.CircleHandleCap);

        if (_monoPath2D.GetNodePosition(i) != newPos)
        {
          if (_selectedNodes.Contains(i))
          {
            Undo.RecordObject(_monoPath2D, "Move nodes");
            var movement = newPos - _monoPath2D.GetNodePosition(i);

            foreach (var index in _selectedNodes)
            {
              _monoPath2D.MovePoint(index, _monoPath2D.GetNodePosition(index) + movement);
            }
          }
          else
          {
            Undo.RecordObject(_monoPath2D, "Move point");
            _monoPath2D.MovePoint(i, newPos);
          }
        }

        switch (anchorType)
        {
          case AnchorStatus.Remove:
            Handles.color = _removeNodeColor;
            Handles.DrawLine(_monoPath2D.GetNodePosition(i) + new Vector2(-1, 1) * (_anchorDiameter * 0.5f),
              _monoPath2D.GetNodePosition(i) + new Vector2(1, -1) * (_anchorDiameter * 0.5f));
            Handles.DrawLine(_monoPath2D.GetNodePosition(i) + new Vector2(-1, -1) * (_anchorDiameter * 0.5f),
              _monoPath2D.GetNodePosition(i) + new Vector2(1, 1) * (_anchorDiameter * 0.5f));
            break;
          case AnchorStatus.Selected:
            Handles.color = _selectedNodeColor;
            Handles.DrawSolidDisc(_monoPath2D.GetNodePosition(i), Vector3.forward, _anchorDiameter * 0.5f);
            break;
          case AnchorStatus.Normal:
          default:
            Handles.color = _nodeColor;
            break;
        }

        Handles.CircleHandleCap(i, _monoPath2D.GetNodePosition(i), Quaternion.identity, _anchorDiameter,
          EventType.Repaint);
      }
    }

    private void DrawSegments()
    {
      for (var i = 0; i < Path2D.SegmentCount; i++)
      {
        var points = _monoPath2D.GetNodesInSegment(i);

        Handles.color = _controlLineColor;
        if (Path2D.GetAnchorType(i * 3) != NodeType.SMOOTH)
        {
          Handles.DrawLine(points[1], points[0]);
        }

        var nextAnchor = (i + 1) * 3;
        if (Path2D.IsClosed && i == Path2D.SegmentCount - 1) nextAnchor = 0;

        if (Path2D.GetAnchorType(nextAnchor) != NodeType.SMOOTH)
        {
          Handles.DrawLine(points[2], points[3]);
        }


        Handles.DrawBezier(points[0], points[3],
          points[1], points[2],
          _selectedSegmentIndex == i && _isCurveDraggable ? _selectedCurveColor : _curveColor, null, 3);
      }
    }

    private Vector2 GetNodePosition(int index)
    {
      if (index < Path2D.NodeCount - 1)
      {
        return index % 3 == 1 ? _monoPath2D.GetNodePosition(index - 1) : _monoPath2D.GetNodePosition(index + 1);
      }

      return _monoPath2D.GetNodePosition(0);
    }

#endregion

#endregion
  }
}