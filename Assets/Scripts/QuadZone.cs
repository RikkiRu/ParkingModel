using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadZone : MonoBehaviour
{
    public Color Color { get; set; }

    private MeshFilter filter;
    private Mesh mesh;

    public void Init(List<Vector2> vertices2D)
    {
        if (mesh != null)
            throw new Exception("QuadZone mesh already inited");

        mesh = new Mesh();
        mesh.name = "QuadZoneMesh";
        filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        MeshUtil.ApplyMaterial(gameObject, Color);

        int[] indices = Triangulator.Triangulate(vertices2D);
        Vector3[] vertices = vertices2D.Select(c => new Vector3(c.x, 0, c.y)).ToArray();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void Init(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        List<Vector2> vertices2D = new List<Vector2>();
        vertices2D.Add(p1);
        vertices2D.Add(p3);
        vertices2D.Add(p4);
        vertices2D.Add(p2);
        Init(vertices2D);
    }
}
