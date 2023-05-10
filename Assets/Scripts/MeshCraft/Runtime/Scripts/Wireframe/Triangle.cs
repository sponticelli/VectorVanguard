using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiteNinja.MeshCraft
{
  /// <summary>
  /// Represents a triangle in 3D space with vertices as edges.
  /// </summary>
  public class Triangle
  {
    private static readonly BarycentricCoords One = new BarycentricCoords(1, 1, 1);
    private static readonly BarycentricCoords Zero = new BarycentricCoords(0, 0, 0);
    private static readonly BarycentricCoords XZ = new BarycentricCoords(1, 0, 1);
    private static readonly BarycentricCoords YZ = new BarycentricCoords(0, 1, 1);
    private static readonly BarycentricCoords Z = new BarycentricCoords(0, 0, 1);

    public static float NormalCoefficient = 1f;
    public static float AngleCoefficient = 1f;
    public static float AreaCoefficient = 0.0f;

    /// <summary>
    /// Index of this triangle in the mesh.
    /// </summary>
    public readonly int Index;
    public readonly Edge edge0;
    public readonly Edge edge1;
    public readonly Edge edge2;
    public BarycentricCoords barycentricCoords0;
    public BarycentricCoords barycentricCoords1;
    public BarycentricCoords barycentricCoords2;
    public readonly Dictionary<Triangle, NeighborInfo> neighbors;

    private readonly Vector3 _normal;
    private readonly float _area;
    private Triangle _match;

    /// <summary>
    /// Initializes a new instance of the Triangle class with specified index, normal, and edges.
    /// </summary>
    /// <param name="index">Index of this triangle in the mesh.</param>
    /// <param name="normal">Normal vector of this triangle.</param>
    /// <param name="edge0">Edge 0 of this triangle.</param>
    /// <param name="edge1">Edge 1 of this triangle.</param>
    /// <param name="edge2">Edge 2 of this triangle.</param>
    public Triangle(int index, Vector3 normal, Edge edge0, Edge edge1, Edge edge2)
    {
      Index = index;
      _normal = normal;
      this.edge0 = edge0;
      this.edge1 = edge1;
      this.edge2 = edge2;
      _area = CalcArea(this);
      neighbors = new Dictionary<Triangle, NeighborInfo>();
      _match = null;
      barycentricCoords0 = new BarycentricCoords(1, 0, 0);
      barycentricCoords1 = new BarycentricCoords(0, 1, 0);
      barycentricCoords2 = new BarycentricCoords(0, 0, 1);
    }


    /// <summary>
    /// Generates information about this triangle's neighbors and stores it in the neighbors dictionary.
    /// </summary>
    public void GenerateNeighborInfo()
    {
      foreach (var neighbor in neighbors)
      {
        var shareEdgeIndexForMe = neighbor.Key.GetSharedEdgeIndexForMe(this);
        if (shareEdgeIndexForMe == -1) continue;
        neighbor.Value.Dot = Vector3.Dot(_normal, neighbor.Key._normal);
        neighbor.Value.Dot = neighbor.Value.Dot * neighbor.Value.Dot * Mathf.Sign(neighbor.Value.Dot);
        neighbor.Value.Dot *= NormalCoefficient;
        var num1 = CalcAngle(this, neighbor.Value.edgeIndex);
        var num2 = num1 <= 90.0 ? num1 / 90f : 90f / num1;
        var num3 = CalcAngle(neighbor.Key, shareEdgeIndexForMe);
        var num4 = num3 <= 90.0 ? num3 / 90f : 90f / num3;
        neighbor.Value.Angle = (float)((num2 + (double)num4) / 2.0);
        neighbor.Value.Angle = Mathf.Lerp(0.0f, neighbor.Value.Angle, AngleCoefficient);
        Vector3 v1;
        Vector3 v2;
        GetEdgeVertices(neighbor.Value.edgeIndex, out v1, out v2);
        var topVertex1 = GetTopVertex(neighbor.Value.edgeIndex);
        var topVertex2 = neighbor.Key.GetTopVertex(shareEdgeIndexForMe);
        var vector3 = topVertex1 - v1;
        var normalized1 = vector3.normalized;
        vector3 = topVertex2 - v1;
        var normalized2 = vector3.normalized;
        var num5 = (float)((Vector3.Dot(normalized1, normalized2) + 1.0) * 90.0);
        var num6 = num5 <= 90.0 ? num5 / 90f : 90f / num5;
        vector3 = topVertex1 - v2;
        var normalized3 = vector3.normalized;
        vector3 = topVertex2 - v2;
        var normalized4 = vector3.normalized;
        var num7 = (float)((Vector3.Dot(normalized3, normalized4) + 1.0) * 90.0);
        var num8 = num7 <= 90.0 ? num7 / 90f : 90f / num7;
        neighbor.Value.Angle += (float)((num6 + (double)num8) / 2.0);
        neighbor.Value.Angle /= 2f;
        if (AreaCoefficient > 0.5)
        {
          var num9 = Vector3.Distance(GetTopVertex(neighbor.Value.edgeIndex),
            neighbor.Key.GetTopVertex(shareEdgeIndexForMe));
          var edgeLength = GetEdgeLength(neighbor.Value.edgeIndex);
          if (edgeLength == 0.0)
            neighbor.Value.Parallel = 0.0f;
          else if (num9 > (double)edgeLength)
            neighbor.Value.Parallel += edgeLength / num9;
          else
            neighbor.Value.Parallel += num9 / edgeLength;
        }

        neighbor.Value.Area = neighbor.Key._area <= (double)_area
          ? neighbor.Key._area / _area
          : _area / neighbor.Key._area;
        neighbor.Value.Area *= AreaCoefficient;
      }
    }


    /// <summary>
    /// Finds the best matching neighbor triangle based on their neighbor information, sets the match connection, and updates their colors.
    /// </summary>
    public void FindBestMatch()
    {
      if (neighbors.Count == 0)
        return;
      var match = (Triangle)null;
      var neighborInfo = (NeighborInfo)null;
      var num1 = -1f;
      foreach (var neighbor in neighbors)
      {
        var num2 = neighbor.Key.GiveMeYourBestNeighbor(out var target);
        if (target == null) continue;
        if (neighbor.Value.Dot > 0.5 && GetFreeNeighbourCount() == 1)
        {
          Debug.Log(Index + " - " + neighbor.Key);
        }
        else if (Index != target.Index) continue;
        if (!(num2 >= (double)num1)) continue;
        num1 = num2;
        match = neighbor.Key;
        neighborInfo = neighbor.Value;
      }

      if (match == null)
        return;
      SetMatchConnection(neighborInfo, match);
    }

    /// <summary>
    /// Sets the match connection between two triangles based on their neighbor info.
    /// Determines the barycentric coords of each triangle and its corresponding vertex based on the connection with its neighbor.
    /// </summary>
    /// <param name="neighborInfo">The neighbor info object containing connection information.</param>
    /// <param name="match">The triangle to connect to.</param>
    private void SetMatchConnection(NeighborInfo neighborInfo, Triangle match)
    {
      barycentricCoords0 = barycentricCoords1 = barycentricCoords2 = match.barycentricCoords0 = match.barycentricCoords1 = match.barycentricCoords2 = Zero;
      switch (neighborInfo.connect0To)
      {
        case 0 when neighborInfo.connect1To == 1:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        case 0 when neighborInfo.connect1To == 2:
          barycentricCoords2 = match.barycentricCoords1 = Z;
          barycentricCoords1 = match.barycentricCoords0 = XZ;
          barycentricCoords0 = match.barycentricCoords2 = YZ;
          break;
        case 0 when neighborInfo.connect2To == 1:
          barycentricCoords1 = match.barycentricCoords2 = Z;
          barycentricCoords2 = match.barycentricCoords0 = XZ;
          barycentricCoords0 = match.barycentricCoords1 = YZ;
          break;
        case 0 when neighborInfo.connect2To == 2:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        case 0:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        case 1 when neighborInfo.connect1To == 0:
          barycentricCoords2 = match.barycentricCoords2 = Z;
          barycentricCoords1 = match.barycentricCoords1 = XZ;
          barycentricCoords0 = match.barycentricCoords0 = YZ;
          break;
        case 1 when neighborInfo.connect1To == 2:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        case 1 when neighborInfo.connect2To == 0:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        case 1 when neighborInfo.connect2To == 2:
          barycentricCoords1 = match.barycentricCoords0 = Z;
          barycentricCoords2 = match.barycentricCoords1 = XZ;
          barycentricCoords0 = match.barycentricCoords2 = YZ;
          break;
        case 1:
          barycentricCoords0 = match.barycentricCoords0 = One;
          barycentricCoords1 = match.barycentricCoords1 = One;
          barycentricCoords2 = match.barycentricCoords2 = One;
          break;
        default:
        {
          switch (neighborInfo.connect1To)
          {
            case 0:
              barycentricCoords0 = match.barycentricCoords1 = Z;
              barycentricCoords2 = match.barycentricCoords0 = XZ;
              barycentricCoords1 = match.barycentricCoords2 = YZ;
              break;
            case 1:
              barycentricCoords2 = match.barycentricCoords0 = Z;
              barycentricCoords1 = match.barycentricCoords2 = XZ;
              barycentricCoords0 = match.barycentricCoords1 = YZ;
              break;
            default:
            {
              switch (neighborInfo.connect2To)
              {
                case 0:
                  barycentricCoords1 = match.barycentricCoords1 = Z;
                  barycentricCoords2 = match.barycentricCoords2 = XZ;
                  barycentricCoords0 = match.barycentricCoords0 = YZ;
                  break;
                case 1:
                  barycentricCoords0 = match.barycentricCoords0 = Z;
                  barycentricCoords2 = match.barycentricCoords2 = XZ;
                  barycentricCoords1 = match.barycentricCoords1 = YZ;
                  break;
                default:
                  barycentricCoords0 = match.barycentricCoords0 = One;
                  barycentricCoords1 = match.barycentricCoords1 = One;
                  barycentricCoords2 = match.barycentricCoords2 = One;
                  break;
              }

              break;
            }
          }

          break;
        }
      }

      _match = match;
      match._match = this;
      neighbors.Clear();
      match.neighbors.Clear();
    }

    /// <summary>
    /// Finds the neighbor with the highest Weight() value among the triangles that do not have a match, and returns its value.
    /// If there are no such triangles, it returns -1.
    /// </summary>
    /// <param name="target">Output parameter that receives the Triangle object representing the best neighbor.</param>
    /// <returns>The Weight() value of the best neighbor, or -1 if there are no such neighbors.</returns>
    public float GiveMeYourBestNeighbor(out Triangle target)
    {
      var maxWeight = -1f;
      target = null;
      if (_match != null)
        return -1f;
      foreach (var neighbor in neighbors.Where(neighbor =>
                 neighbor.Key is { _match: null } && neighbor.Value.Weight() > (double)maxWeight))
      {
        maxWeight = neighbor.Value.Weight();
        target = neighbor.Key;
      }

      return maxWeight;
    }


    /// <summary>
    /// Determines the index of the edge that is shared between the current triangle and another triangle.
    /// </summary>
    /// <param name="other">The other triangle to compare with.</param>
    /// <returns>The index of the shared edge, or -1 if there is no shared edge.</returns>
    private int GetSharedEdgeIndexForMe(Triangle other)
    {
      if (other == null || this == other) return -1;
      foreach (var neighbor in
               neighbors.Where(neighbor => neighbor.Key == other))
      {
        return neighbor.Value.edgeIndex;
      }

      return -1;
    }

    /// <summary>
    /// Gets the index of the shared edge for the specified triangle <paramref name="other"/> that is also shared by this triangle.
    /// Returns -1 if there is no shared edge.
    /// </summary>
    /// <param name="other">The other triangle to check for a shared edge.</param>
    /// <returns>The index of the shared edge for the specified triangle that is also shared by this triangle,
    /// or -1 if there is no shared edge.</returns>
    private int GetShareEdgeIndexForYou(Triangle other)
    {
      if (other == null || this == other) return -1;
      foreach (var neighbor in
               other.neighbors.Where(neighbor => neighbor.Key == this))
      {
        return neighbor.Value.edgeIndex;
      }

      return -1;
    }

    /// <summary>
    /// Gets the count of free neighboring triangles.
    /// </summary>
    /// <returns>The number of neighboring triangles that are not matched with another triangle.</returns>
    private int GetFreeNeighbourCount()
    {
      var num = neighbors.Count(neighbor => neighbor.Key is { _match: null });

      return 0;
    }

    /// <summary>
    /// Given an edge index, returns the two edges that compound that edge. 
    /// </summary>
    /// <param name="edgeIndex">The index of the edge to be compounded</param>
    /// <param name="edge1">Out parameter to store the first edge that compounds the given edge</param>
    /// <param name="edge2">Out parameter to store the second edge that compounds the given edge</param>
    private void GetCompoundEdges(int edgeIndex, out Edge edge1, out Edge edge2)
    {
      switch (edgeIndex)
      {
        case 0:
          edge1 = this.edge1;
          edge2 = this.edge2;
          break;
        case 1:
          edge1 = edge0;
          edge2 = this.edge2;
          break;
        case 2:
          edge1 = edge0;
          edge2 = this.edge1;
          break;
        default:
          edge1 = this.edge1;
          edge2 = this.edge2;
          break;
      }
    }

    /// <summary>
    /// Gets the vertex at the top of the specified edge index.
    /// </summary>
    /// <param name="edgeIndex">The index of the edge to get the top vertex of.</param>
    /// <returns>The vertex at the top of the specified edge.</returns>
    private Vector3 GetTopVertex(int edgeIndex)
    {
      return edgeIndex switch
      {
        0 => edge2.v1.position,
        1 => edge0.v1.position,
        2 => edge1.v1.position,
        _ => Vector3.zero
      };
    }

    /// <summary>
    /// Gets the length at the top of the specified edge index.
    /// </summary>
    /// <param name="edgeIndex">The index of the edge to get the top vertex of.</param>
    /// <returns>The length of the specified edge.</returns>
    private float GetEdgeLength(int edgeIndex)
    {
      return edgeIndex switch
      {
        0 => edge0.length,
        1 => edge1.length,
        2 => edge2.length,
        _ => 0.0f
      };
    }

    /// <summary>
    /// Returns the two vertices of the edge with the given index.
    /// </summary>
    /// <param name="edgeIndex">The index of the edge (0, 1, or 2).</param>
    /// <param name="v1">Output parameter for the first vertex of the edge.</param>
    /// <param name="v2">Output parameter for the second vertex of the edge.</param>
    private void GetEdgeVertices(int edgeIndex, out Vector3 v1, out Vector3 v2)
    {
      switch (edgeIndex)
      {
        case 0:
          v1 = edge0.v1.position;
          v2 = edge0.v2.position;
          break;
        case 1:
          v1 = edge1.v1.position;
          v2 = edge1.v2.position;
          break;
        case 2:
          v1 = edge2.v1.position;
          v2 = edge2.v2.position;
          break;
        default:
          v1 = Vector3.zero;
          v2 = Vector3.zero;
          break;
      }
    }


    /// <summary>
    /// Calculates the parallel coefficient between two edges of two triangles.
    /// </summary>
    /// <param name="t1">The first triangle.</param>
    /// <param name="t1EI">The index of the edge in the first triangle.</param>
    /// <param name="t2">The second triangle.</param>
    /// <param name="t2EI">The index of the edge in the second triangle.</param>
    /// <returns>The calculated parallel coefficient.</returns>
    private static float CalaParallelCoefficient(Triangle t1, int t1EI, Triangle t2, int t2EI)
    {
      t1.GetCompoundEdges(t1EI, out var e11, out var e21);
      t2.GetCompoundEdges(t2EI, out var e12, out var e22);
      return !e11.DoShareVertex(e12)
        ? Mathf.Abs(Vector3.Dot(e11.normalizedDirection, e12.normalizedDirection)) +
          Mathf.Abs(Vector3.Dot(e21.normalizedDirection, e22.normalizedDirection))
        : Mathf.Abs(Vector3.Dot(e11.normalizedDirection, e22.normalizedDirection)) +
          Mathf.Abs(Vector3.Dot(e21.normalizedDirection, e12.normalizedDirection));
    }


    /// <summary>
    /// Calculates the angle in degrees between the edge of a triangle and the vector defined by the shared vertex and the opposite vertex of the edge.
    /// </summary>
    /// <param name="triangle">The triangle to calculate the angle for.</param>
    /// <param name="edgeIndex">The index of the edge to calculate the angle for (0, 1 or 2).</param>
    /// <returns>The angle in degrees between the edge and the vector defined by the shared vertex and the opposite vertex of the edge.</returns>
    private static float CalcAngle(Triangle triangle, int edgeIndex)
    {
      Vector3 vector31;
      Vector3 vector32;
      Vector3 vector33;
      switch (edgeIndex)
      {
        case 0:
          vector31 = triangle.edge2.v1.position;
          vector32 = triangle.edge0.v1.position;
          vector33 = triangle.edge0.v2.position;
          break;
        case 1:
          vector31 = triangle.edge0.v1.position;
          vector32 = triangle.edge1.v1.position;
          vector33 = triangle.edge1.v2.position;
          break;
        case 2:
          vector31 = triangle.edge1.v1.position;
          vector32 = triangle.edge2.v1.position;
          vector33 = triangle.edge2.v2.position;
          break;
        default:
          Debug.LogError(triangle.Index + " - " + edgeIndex);
          vector31 = Vector3.zero;
          vector32 = Vector3.up;
          vector33 = Vector3.down;
          break;
      }

      return (float)((Vector3.Dot((vector32 - vector31).normalized, (vector33 - vector31).normalized) +
                      1.0) * 90.0);
    }

    /// <summary>
    /// Calculates the area of a triangle using its edges.
    /// </summary>
    /// <param name="triangle">The triangle to calculate the area of.</param>
    /// <returns>The area of the triangle.</returns>
    private static float CalcArea(Triangle triangle)
    {
      var vector3 = triangle.edge0.v1 - triangle.edge0.v2;
      var normalized1 = vector3.normalized;
      vector3 = triangle.edge1.v2 - triangle.edge1.v1;
      var normalized2 = vector3.normalized;
      var num = Vector3.Angle(normalized1, normalized2);
      return 0.5f * Vector3.Distance(triangle.edge0.v1.position, triangle.edge0.v2.position) *
             Vector3.Distance(triangle.edge1.v2.position, triangle.edge1.v1.position) *
             Mathf.Sin((float)Math.PI / 180f * num);
    }

    public override string ToString() =>
      "Index: " + Index + " " + edge0 + ", " + edge1 + ", " + edge2;

    public string ToStringNeighbors()
    {
      if (neighbors == null) return "null";
      var str = Index + " (";
      str = neighbors.Aggregate(str,
        (current, neighbor) => neighbor.Key != null ? current + neighbor.Key.Index + "," : current + "null, ");
      return str[..str.LastIndexOf(',')] + ")";
    }

    public string ToStringNeighborsMass()
    {
      if (neighbors == null) return "null";
      var stringNeighborsMass = Index + "\n";
      foreach (var neighbor in neighbors)
      {
        if (neighbor.Key == null)
          stringNeighborsMass += "null, ";
        else
          stringNeighborsMass = stringNeighborsMass + neighbor.Key.Index + " [dot: " +
                                neighbor.Value.Dot.ToString("f3") + ", angle: " + neighbor.Value.Angle.ToString("f3") +
                                ", parallel: " + neighbor.Value.Parallel.ToString("f3") + ", area: " +
                                neighbor.Value.Area.ToString("f3") + "] FM: " + neighbor.Value.Weight() +
                                "\n";
      }

      return stringNeighborsMass;
    }
  }
}