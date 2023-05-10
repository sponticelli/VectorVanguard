using UnityEngine;

namespace LiteNinja.MeshCraft
{
  public static class MeshHelper
  {
    public static Mesh GetMesh(GameObject gameObject)
    {
      var meshFilter = gameObject.GetComponent<MeshFilter>();
      if (meshFilter != null)
      {
        return meshFilter.sharedMesh;
      }

      var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
      return skinnedMeshRenderer != null ? skinnedMeshRenderer.sharedMesh : null;
    }
    
    public static string MeshToString(Mesh mesh)
    {
      var sb = new System.Text.StringBuilder();

      sb.Append("g ").Append(mesh.name).Append("\n");

      foreach (var v in mesh.vertices)
      {
        sb.Append($"v {v.x} {v.y} {v.z}\n");
      }

      foreach (var n in mesh.normals)
      {
        sb.Append($"vn {n.x} {n.y} {n.z}\n");
      }

      foreach (var uv in mesh.uv)
      {
        sb.Append($"vt {uv.x} {uv.y}\n");
      }

      for (var i = 0; i < mesh.subMeshCount; i++)
      {
        var triangles = mesh.GetTriangles(i);

        for (var j = 0; j < triangles.Length; j += 3)
        {
          sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
            triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
        }
      }

      return sb.ToString();
    }
  }
}