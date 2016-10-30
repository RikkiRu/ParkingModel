using UnityEngine;

public class MeshUtil
{
    public static void CreateMesh(GameObject plane, float width, float height)
    {
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        m.vertices = new Vector3[]
        {
         new Vector3(-width, -height, 0.01f),
         new Vector3(width, -height, 0.01f),
         new Vector3(width, height, 0.01f),
         new Vector3(-width, height, 0.01f)
        };

        m.uv = new Vector2[] 
        {
         new Vector2 (0, 0),
         new Vector2 (0, 1),
         new Vector2(1, 1),
         new Vector2 (1, 0)
        };

        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();

        meshFilter.mesh = m;
    }

    public static void ApplyMaterial(GameObject plane, Color color)
    {
        MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material.shader = Shader.Find("Sprites/Default");
        renderer.material.color = color;
    }

    public static GameObject MakeSolidMesh(string name, float sizeX, float sizeY, Color32 color)
    {
        GameObject plane = new GameObject(name);
        CreateMesh(plane, sizeX, sizeY);
        ApplyMaterial(plane, color);
        return plane;
    }
}
