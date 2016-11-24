using UnityEngine;
using System.Collections.Generic;

public class PathNode : MonoBehaviour
{
    private const float DirScale = 1.5f;        // Расширение дороги по напр. веектору
    private const float NormScale = 2.0f;       // Расширение дороги по вектору нормали
    private const float MagenetD = 2f;        // Дистанция слипания точек

    [SerializeField] QuadZone quadZonePrefab;

    public bool UserNode { get; set; }
    public List<PathNode> OutNodes { get; set; }
    private List<GameObject> objects;
    private Transform objectsHolder;

    public List<Vector2> magnetPoints;

    private void Awake()
    {
        OutNodes = new List<PathNode>();
        objects = new List<GameObject>();
        magnetPoints = new List<Vector2>();

        GameObject lineHolderObj = new GameObject("LineHolder");
        lineHolderObj.transform.SetParent(transform, false);
        objectsHolder = lineHolderObj.transform;
    }

    public void CheckNodeForRemove(PathNode node)
    {
        if (OutNodes.Contains(node))
        {
            OutNodes.Remove(node);
            ReDraw();
        }
    }

    public void AddNode(PathNode node)
    {
        if (OutNodes.Contains(node))
            return;

        OutNodes.Add(node);
        ReDraw();
    }

    private void CleanObjects()
    {
        foreach (var i in objects)
            Destroy(i);

        objects.Clear();
    }

    private void OnDestroy()
    {
        CleanObjects();
    }

    private void ReDraw()
    {
        CleanObjects();

        foreach (var i in OutNodes)
        {
            Vector2 p1 = new Vector2(transform.position.x, transform.position.z);
            Vector2 p2 = new Vector2(i.transform.position.x, i.transform.position.z);

            GeometryUtil.LineOptions lineOpt = new GeometryUtil.LineOptions(p1, p2);

            float dNormScale = NormScale / Mathf.Sqrt(Mathf.Pow(lineOpt.NormX, 2) + Mathf.Pow(lineOpt.NormY, 2));

            Vector2 s1 = p1;
            Vector2 s2 = p2;

            float dirX = lineOpt.AbsDirX * DirScale;
            float dirY = lineOpt.AbsDirY * DirScale;

            if (s1.x < s2.x)
                dirX = -dirX;

            if (s1.y < s2.y)
                dirY = -dirY;

            s1 = new Vector2(s1.x + dirX, s1.y + dirY);
            s2 = new Vector2(s2.x - dirX, s2.y - dirY);

            Vector2 q1 = new Vector2(s1.x - lineOpt.NormX * dNormScale, s1.y - lineOpt.NormY * dNormScale);
            Vector2 q2 = new Vector2(s1.x + lineOpt.NormX * dNormScale, s1.y + lineOpt.NormY * dNormScale);
            Vector2 q3 = new Vector2(s2.x - lineOpt.NormX * dNormScale, s2.y - lineOpt.NormY * dNormScale);
            Vector2 q4 = new Vector2(s2.x + lineOpt.NormX * dNormScale, s2.y + lineOpt.NormY * dNormScale);

            CorrectWithMagnetPoints(ref q1, ref q2);
            i.CorrectWithMagnetPoints(ref q3, ref q4);

            var quad = Instantiate(quadZonePrefab);
            MapCreatorLoader.Instance.Attach(quad.gameObject);
            quad.UpdateMesh(q1, q2, q3, q4);
            var quadRender = quad.gameObject.GetComponent<MeshRenderer>();
            quadRender.sortingOrder = 30;
            objects.Add(quad.gameObject);

            var line = MakeLine(GeometryUtil.V3(p1), GeometryUtil.V3(p2), Color.black);
            var lineRender = line.GetComponent<LineRenderer>();
            lineRender.sortingOrder = 40;
            objects.Add(line);
        }
    }

    public void CorrectWithMagnetPoints(ref Vector2 distation1, ref Vector2 distation2)
    {
        bool wasChanges1 = false;
        bool wasChanges2 = false;

        float minDist1 = float.MaxValue;
        float minDist2 = float.MaxValue;

        Vector2 closest1 = Vector2.zero;
        Vector2 closest2 = Vector2.zero;

        foreach (var i in magnetPoints)
        {
            float d1 = Vector2.Distance(distation1, i);
            float d2 = Vector2.Distance(distation2, i);

            if (d1 < minDist1)
            {
                wasChanges1 = true;
                minDist1 = d1;
                closest1 = i;
            }

            if (d2 < minDist2)
            {
                wasChanges2 = true;
                minDist2 = d2;
                closest2 = i;
            }
        }

        if (!wasChanges1 || minDist1 > MagenetD)
            magnetPoints.Add(distation1);
        else
            distation1 = closest1;

        if (!wasChanges2 || minDist2 > MagenetD)
            magnetPoints.Add(distation2);
        else
            distation2 = closest2;
    }

    private GameObject MakeLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject("line");
        myLine.transform.SetParent(objectsHolder, false);
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(MeshUtil.ShaderSprite);
        lr.SetColors(color, color);
        lr.SetWidth(0.2f, 0.2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }
}