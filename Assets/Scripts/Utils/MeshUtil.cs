using UnityEngine;

public class MeshUtil
{
    private static Shader shaderSprite;

    public static Shader ShaderSprite
    {
        get
        {
            if (shaderSprite == null)
                shaderSprite = Shader.Find("Sprites/Default");

            return shaderSprite;
        }
    }

    public static MeshFilter CreateMesh(GameObject plane, float width, float height)
    {
        MeshFilter meshFilter = plane.AddComponent<MeshFilter>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        float width2 = width / 2;
        float height2 = height / 2;

        m.vertices = new Vector3[]
        {
         new Vector3(-width2, 0, -height2),
         new Vector3(width2, 0, -height2),
         new Vector3(-width2, 0, height2),
         new Vector3(width2, 0, height2),
        };

        m.uv = new Vector2[] 
        {
         new Vector2 (0, 0),
         new Vector2 (1, 0),
         new Vector2 (0, 1),
         new Vector2 (1, 1),
        };

        m.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        m.RecalculateNormals();
        m.Optimize();

        meshFilter.mesh = m;

        return meshFilter;
    }

    public static MeshRenderer ApplyMaterial(GameObject plane, Color color)
    {
        MeshRenderer renderer = plane.AddComponent<MeshRenderer>();
        renderer.material.shader = ShaderSprite;
        renderer.material.color = color;
        return renderer;
    }

    public static GameObject MakeSolidMesh(string name, float sizeX, float sizeY, Color32 color)
    {
        GameObject plane = new GameObject(name);
        CreateMesh(plane, sizeX, sizeY);
        ApplyMaterial(plane, color);
        return plane;
    }
}
