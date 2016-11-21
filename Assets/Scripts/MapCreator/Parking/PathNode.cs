using UnityEngine;
using System.Collections.Generic;

public class PathNode : MonoBehaviour
{
    public bool UserNode { get; set; }
    public List<PathNode> OutNodes { get; set; }
    private List<GameObject> lines;
    private Transform lineHolder;

    private void Awake()
    {
        OutNodes = new List<PathNode>();
        lines = new List<GameObject>();

        GameObject lineHolderObj = new GameObject("LineHolder");
        lineHolderObj.transform.SetParent(transform, false);
        lineHolder = lineHolderObj.transform;
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
        foreach (var i in lines)
            Destroy(i);

        lines.Clear();

        foreach (var i in OutNodes)
        {
            var line = MakeLine(transform.position, i.transform.position, Color.black);
            lines.Add(line);
        }
    }

    private GameObject MakeLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.SetParent(lineHolder, false);
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.5f, 0.5f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;
    }
}