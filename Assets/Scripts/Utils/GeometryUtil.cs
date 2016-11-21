using System.Collections.Generic;
using UnityEngine;

public class GeometryUtil
{
    public static bool TrigangleIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 x)
    {
        float bx = p2.x - p1.x;
        float by = p2.y - p1.y;
        float cx = p3.x - p1.x;
        float cy = p3.y - p1.y;
        float px = x.x - p1.x;
        float py = x.y - p1.y;

        float m = (px * by - bx * py) / (cx * by - bx * cy);

        if (m >= 0 && m <= 1)
        {
            float l = (px - m * cx) / bx;
            if (l >= 0 && (m + l) <= 1)
                return true;
        }

        return false;
    }

    public static bool Pnpoly(Vector2[] vert, Vector2 test)
    {
        int nvert = vert.Length;
        bool c = false;

        int i, j;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (((vert[i].y > test.y) != (vert[j].y > test.y)) &&
             (test.x < (vert[j].x - vert[i].x) * (test.y - vert[i].y) / (vert[j].y - vert[i].y) + vert[i].x))
                c = !c;
        }

        return c;
    }

    public static int FindClosest<T>(List<T> list, Vector2 p) where T : MonoBehaviour
    {
        float d;
        return FindClosest(list, p, out d);
    }

    public static int FindClosest<T>(List<T> list, Vector2 p, out float outDist) where T : MonoBehaviour
    {
        int indxMinDist = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            Vector3 pos = list[i].transform.position;
            Vector2 check = new Vector2(pos.x, pos.z);
            float dist = Vector2.Distance(check, p);

            if (dist < minDist)
            {
                indxMinDist = i;
                minDist = dist;
            }
        }

        outDist = minDist;
        return indxMinDist;
    }

    public static int FindClosest(List<Vector2> list, Vector2 p)
    {
        int indxMinDist = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            Vector2 check = list[i];
            float dist = Vector2.Distance(check, p);

            if (dist < minDist)
            {
                indxMinDist = i;
                minDist = dist;
            }
        }

        return indxMinDist;
    }
}