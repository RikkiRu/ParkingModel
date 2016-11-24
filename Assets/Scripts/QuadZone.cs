using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadZone : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;

    private void Awake()
    {
        //filter = MeshUtil.CreateMesh(gameObject, 10, 10);
        //MeshUtil.ApplyMaterial(gameObject, Color.gray);
    }

    public void UpdateMesh(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        List<Vector2> vertices2D = new List<Vector2>();
        vertices2D.Add(p1);
        vertices2D.Add(p3);
        vertices2D.Add(p4);
        vertices2D.Add(p2);

        if (mesh == null)
            mesh = new Mesh();

        mesh.name = "QuadZoneMesh";

        if (filter == null)
            filter = gameObject.AddComponent<MeshFilter>();

        filter.mesh = mesh;

        MeshUtil.ApplyMaterial(gameObject, new Color32(210, 210, 210, 255));

        int[] indices = Triangulator.Triangulate(vertices2D);
        Vector3[] vertices = vertices2D.Select(c => new Vector3(c.x, 0, c.y)).ToArray();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
