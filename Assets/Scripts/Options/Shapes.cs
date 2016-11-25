using UnityEngine;
using System.Collections.Generic;

public enum Shape
{
    None,
    Edge8,
}

public class Shapes
{
    private static Dictionary<Shape, Vector2[]> presets = null; 

    private static void Init()
    {
        presets = new Dictionary<Shape, Vector2[]>();
        presets.Add(Shape.Edge8, Edge8());
    }

    public static List<Vector2> Get(Shape shapeType, float size, float px, float py)
    {
        if (presets == null)
            Init();

        Vector2[] offsets = presets[shapeType];

        List<Vector2> sized = new List<Vector2>();
        foreach (var o in offsets)
        {
            Vector2 v = new Vector2(o.x * size + px, o.y * size + py);
            sized.Add(v);
        }

        return sized;
    }

    private static Vector2[] Edge8()
    {
        const float P = 0.7f;

        Vector2[] offsets = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(P, P),
            new Vector2(1, 0),
            new Vector2(P, -P),
            new Vector2(0, -1),
            new Vector2(-P, -P),
            new Vector2(-1, 0),
            new Vector2(-P, P),
        };

        return offsets;
    }
}