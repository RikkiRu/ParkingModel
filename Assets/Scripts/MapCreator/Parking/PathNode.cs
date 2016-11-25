using UnityEngine;
using System.Collections.Generic;
using System;

public class PathNode : MonoBehaviour
{
    private const float NormScale = 2.0f;       // Расширение дороги по вектору нормали
    private const float NodeBaseScale = 2f;     // Скейл платформы

    [SerializeField] QuadZone quadZonePrefab;

    public List<PathNode> OutNodes { get; set; }
    public List<Vector2> MagnetPoints { get; set; }
    private List<GameObject> Objects { get; set; }
    private GameObject PlatformObject { get; set; }
    private Transform ObjectsHolder { get; set; }

    private void Awake()
    {
        OutNodes = new List<PathNode>();
        Objects = new List<GameObject>();
        MagnetPoints = null;

        GameObject lineHolderObj = new GameObject("NodeChilds");
        lineHolderObj.transform.SetParent(transform, false);
        ObjectsHolder = lineHolderObj.transform;
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
        foreach (var i in Objects)
            Destroy(i);

        Objects.Clear();
    }

    private void OnDestroy()
    {
        if (PlatformObject != null)
        {
            Destroy(PlatformObject);
            PlatformObject = null;
        }

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

            Vector2 q1 = new Vector2(s1.x - lineOpt.NormX * dNormScale, s1.y - lineOpt.NormY * dNormScale);
            Vector2 q2 = new Vector2(s1.x + lineOpt.NormX * dNormScale, s1.y + lineOpt.NormY * dNormScale);
            Vector2 q3 = new Vector2(s2.x - lineOpt.NormX * dNormScale, s2.y - lineOpt.NormY * dNormScale);
            Vector2 q4 = new Vector2(s2.x + lineOpt.NormX * dNormScale, s2.y + lineOpt.NormY * dNormScale);

            CorrectWithMagnetPoints(ref q1, ref q2);
            i.CorrectWithMagnetPoints(ref q3, ref q4);

            var quad = Instantiate(quadZonePrefab);
            MapCreatorLoader.Instance.Attach(quad.gameObject);
            quad.Color = Colors.RoadColor;
            quad.Init(q1, q2, q3, q4);
            var quadRender = quad.gameObject.GetComponent<MeshRenderer>();
            quadRender.sortingOrder = SortingOrder.Is(Layer.Road);
            Objects.Add(quad.gameObject);

            var line = MakeLine(GeometryUtil.V3(p1), GeometryUtil.V3(p2), Colors.NodeLineColor);
            var lineRender = line.GetComponent<LineRenderer>();
            lineRender.sortingOrder = SortingOrder.Is(Layer.NodeLine);
            Objects.Add(line);
        }
    }

    private MagnetInfo GetMagnetPointsInfo(Vector2 distation)
    {
        MagnetInfo info = new MagnetInfo();

        foreach (var i in MagnetPoints)
        {
            float dist = Vector2.Distance(distation, i);

            if (dist < info.MinDist)
            {
                info.WasChanges = true;
                info.MinDist = dist;
                info.Closest = i;
            }
        }

        return info;
    }

    private void ProcessMagnetPointsInfo(MagnetInfo info, ref Vector2 distation)
    {
        if (!info.WasChanges)
            MagnetPoints.Add(distation);
        else
            distation = info.Closest;
    }

    public void CorrectWithMagnetPoints(ref Vector2 distation1, ref Vector2 distation2)
    {
        var info1 = GetMagnetPointsInfo(distation1);
        var info2 = GetMagnetPointsInfo(distation2);
        ProcessMagnetPointsInfo(info1, ref distation1);
        ProcessMagnetPointsInfo(info2, ref distation2);
    }

    private GameObject MakeLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject("line");
        myLine.transform.SetParent(ObjectsHolder, false);
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(MeshUtil.ShaderSprite);
        lr.SetColors(color, color);
        lr.SetWidth(0.2f, 0.2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }

    public void SetPosition(Vector3 p3d)
    {
        transform.localPosition = p3d;

        var shape = Shapes.Get(Shape.Edge8, NodeBaseScale, transform.localPosition.x, transform.localPosition.z);
        var platform = Instantiate(quadZonePrefab);
        platform.name = "NodeBase";
        MapCreatorLoader.Instance.Attach(platform.gameObject);
        platform.Color = Colors.RoadColor;
        platform.Init(shape);
        var platformRender = platform.gameObject.GetComponent<MeshRenderer>();
        platformRender.sortingOrder = SortingOrder.Is(Layer.NodeBase);
        PlatformObject = platform.gameObject;

        MagnetPoints = new List<Vector2>();
        foreach (var i in shape)
            MagnetPoints.Add(i);

        ReDraw();
    }

    private class MagnetInfo
    {
        public bool WasChanges { get; set; }
        public float MinDist { get; set; }
        public Vector2 Closest { get; set; }

        public MagnetInfo()
        {
            WasChanges = false;
            MinDist = float.MaxValue;
            Closest = Vector2.zero;
        }
    }
}