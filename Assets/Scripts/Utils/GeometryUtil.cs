using System;
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

    public static Vector2 VectAvarage(Vector2 v1, Vector2 v2)
    {
        float x = (v1.x + v2.x) / 2;
        float y = (v1.y + v2.y) / 2;
        x = (float)Math.Round(x, 2);
        y = (float)Math.Round(y, 2);

        return new Vector2(x, y);
    }

    public static Vector2 V2 (Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 V3 (Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public class LineOptions
    {
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        public float NormX { get; set; }
        public float NormY { get; set; }
        public float D_NormX { get; set; }
        public float D_NormY { get; set; }

        public float DirX { get; set; } // -B
        public float DirY { get; set; } // +A
        public float D_DirX { get; set; }
        public float D_DirY { get; set; }

        public LineOptions(Vector2 p1, Vector2 p2)
        {
            float dy = p2.y - p1.y;
            float dx = p2.x - p1.x;

            A = dy;
            B = -dx;
            C = p1.y * dx - p1.x * dy;

            if (A == 0 && B == 0)
                throw new Exception("Look like line is not line - it's point!");

            float normX = A;
            float normY = B;
            Normilize(ref normX, ref normY);
            NormX = normX;
            NormY = normY;

            float dirX = -B;
            float dirY = +A;
            Normilize(ref dirX, ref dirY);
            DirX = dirX;
            DirY = dirY;
        }

        public void CalcNormScale(float scale)
        {
            // Вектор пронормирован. Получается что при диагональном смещении 
            // мы смещаем большее смещение чем нужно, поэтому надо уменьшить
            // норму в соответствии с её реальным размером

            float dNormScale = scale / Mathf.Sqrt(Mathf.Pow(NormX, 2) + Mathf.Pow(NormY, 2));
            D_NormX = NormX * dNormScale;
            D_NormY = NormY * dNormScale;
        }

        public void CalcDirScale(float scale)
        {
            float dDirScale = scale / Mathf.Sqrt(Mathf.Pow(DirX, 2) + Mathf.Pow(DirY, 2));
            D_DirX = DirX * dDirScale;
            D_DirY = DirY * dDirScale;
        }

        public Vector2 GetDirOffset(Vector2 from, float dist, float direction)
        {
            float delta = dist * direction;
            float nx = from.x + D_DirX * delta;
            float ny = from.y + D_DirY * delta;
            return new Vector2(nx, ny);
        }

        public Vector2 MakeNormalOffset(Vector2 point, float direction)
        {
            return new Vector2(point.x + direction * D_NormX, point.y + direction * D_NormY);
        }

        private void Normilize(ref float x, ref float y)
        {
            float kX = x > 0 ? 1 : -1;
            float kY = y > 0 ? 1 : -1;

            if (Mathf.Abs(x) > Math.Abs(y))
            {
                y = Math.Abs(y / x);
                x = 1;
            }
            else
            {
                x = Math.Abs(x / y);
                y = 1;
            }

            x = x * kX;
            y = y * kY;
        }
    }
}