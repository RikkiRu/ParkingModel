using System.Collections.Generic;
using UnityEngine;

public class GeneticEssence : MonoBehaviour
{
    private const int MaxNodesCount = 20;
    private const int MinNodesCount = 3;
    private const int MaxConnections = MaxNodesCount * 2;

    public List<NodeInf> Nodes { get; set; }
    public int IdCounter { get; set; }

    private GeneticController Controller { get { return GeneticController.Instance; } }

    public GeneticEssence()
    {
        Nodes = new List<NodeInf>();
        IdCounter = 0;

        RandomNodes();
        RandomConnections();
    }

    public GeneticEssence(GeneticEssence parent)
    {
        Nodes = new List<NodeInf>();
        IdCounter = parent.IdCounter;

        foreach(var i in parent.Nodes)
            Nodes.Add(i.Clone());
    }

    private void AddNewNode()
    {
        NodeInf inf = new NodeInf();
        float x = Random.Range(Controller.BoundsMin.x, Controller.BoundsMax.x);
        float y = Random.Range(Controller.BoundsMin.y, Controller.BoundsMax.y);
        inf.Position = new Vector2(x, y);
        inf.ID = IdCounter;
        IdCounter++;
        inf.OutConnections = new List<int>();
        inf.InConnections = new List<int>();
        Nodes.Add(inf);
    }

    private void RandomNodes()
    {
        int nCount = Random.Range(MinNodesCount, MaxNodesCount + 1);
        for (int i = 0; i < nCount; i++)
            AddNewNode();
    }

    private void RandomConnections()
    {

    }

    public class NodeInf
    {
        public Vector2 Position { get; set; }
        public int ID { get; set; }
        public List<int> OutConnections { get; set; }
        public List<int> InConnections { get; set; }

        public NodeInf Clone()
        {
            NodeInf clone = new NodeInf();
            clone.ID = ID;
            clone.Position = Position;

            clone.OutConnections = new List<int>();
            foreach (var i in OutConnections)
                clone.OutConnections.Add(i);

            clone.InConnections = new List<int>();
            foreach (var i in InConnections)
                clone.InConnections.Add(i);

            return clone;
        }
    }
}