using UnityEngine;
using System.Collections.Generic;

public class PathNode : MonoBehaviour
{
    [SerializeField] QuadZone quadZonePrefab;

    public bool UserNode { get; set; }
    public List<PathNode> OutNodes { get; set; }
    private List<GameObject> objects;
    private Transform objectsHolder;

    private void Awake()
    {
        OutNodes = new List<PathNode>();
        objects = new List<GameObject>();

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
        OutNodes.Add(node);
        ReDraw();
    }

    private void ReDraw()
    {
        foreach (var i in objects)
            Destroy(i);

        objects.Clear();

        foreach (var i in OutNodes)
        {
            Vector2 p1 = new Vector2(transform.position.x, transform.position.z);
            Vector2 p2 = new Vector2(i.transform.position.x, i.transform.position.z);
            Vector2 pA = GeometryUtil.VectAvarage(p1, p2);

            var quad = Instantiate(quadZonePrefab);
            //quad.transform.SetParent(objectsHolder, false);
            MapCreatorLoader.Instance.Attach(quad.gameObject);
            //quad.transform.position = GeometryUtil.V3(pA);
            
            const float naprScale = 2;
            const float normScale = 2;

            GeometryUtil.LineOptions lineOpt = new GeometryUtil.LineOptions(p1, p2);

            float dNormScale = normScale / Mathf.Sqrt(Mathf.Pow(lineOpt.NormX, 2) + Mathf.Pow(lineOpt.NormY, 2));

            Debug.Log("NormX: " + lineOpt.NormX);
            Debug.Log("NormY: " + lineOpt.NormY);
            Vector2 q1 = new Vector2(p1.x - lineOpt.NormX * dNormScale, p1.y - lineOpt.NormY * dNormScale);
            Vector2 q2 = new Vector2(p1.x + lineOpt.NormX * dNormScale, p1.y + lineOpt.NormY * dNormScale);
            Vector2 q3 = new Vector2(p2.x - lineOpt.NormX * dNormScale, p2.y - lineOpt.NormY * dNormScale);
            Vector2 q4 = new Vector2(p2.x + lineOpt.NormX * dNormScale, p2.y + lineOpt.NormY * dNormScale);
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