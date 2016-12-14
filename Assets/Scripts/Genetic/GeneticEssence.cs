using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticEssence
{
    private const int MaxNodesCount = 20;
    private const int MinNodesCount = 3;
    private const int MaxConnections = MaxNodesCount * 2;

    public List<NodeInf> Nodes { get; set; }
    public int IdCounter { get; set; }

    private GeneticController Controller { get { return GeneticController.Instance; } }

    public GeneticEssence(List<PathNode> userNodes)
    {
        Nodes = new List<NodeInf>();
        IdCounter = 0;

        MakeUserNodes(userNodes);
        RandomNodes();
    }

    public GeneticEssence(GeneticEssence parent)
    {
        Nodes = new List<NodeInf>();
        IdCounter = parent.IdCounter;

        foreach(var i in parent.Nodes)
            Nodes.Add(i.Clone());
    }

    private void MakeUserNodes(List<PathNode> userNodes)
    {
        foreach (var node in userNodes)
        {
            NodeInf inf = new NodeInf();
            inf.Position = node.XZ;
            inf.ID = IdCounter;
            IdCounter++;
            inf.OutConnections = new List<int>();
            inf.InConnections = new List<int>();
            inf.SourceNode = true;
            Nodes.Add(inf);
        }
    }

    private NodeInf GenerateNewNode(NodeInf parent, float dist)
    {
        NodeInf inf = new NodeInf();

        float x = 0;
        float y = 0;
        int iterations = 0;
        Vector2 nPos = Vector2.zero;
        float distToParent = float.MaxValue;
        int closeNodes = 0;
        int maxCloseNodes = 1;

        do
        {
            iterations++;
            if (iterations > 15)
                return null;

            int kX = Random.Range(-1, 2);
            int kY = Random.Range(-1, 2);

            x = kX * dist;
            y = kY * dist;

            nPos = parent.Position + new Vector2(x, y);

            closeNodes = 0;
            foreach (var i in Nodes)
            {
                float d = Vector2.Distance(i.Position, nPos);
                if (d < 1)
                    closeNodes++;

                if (closeNodes > maxCloseNodes)
                    break;
            }

            distToParent = Vector2.Distance(parent.Position, nPos);
        }
        while (closeNodes > maxCloseNodes || distToParent < 1 || !MapCreatorLoader.Instance.ParkingZone.CanPlaceTo(nPos));

        inf.Position = nPos;
        inf.ID = IdCounter;
        IdCounter++;
        inf.OutConnections = new List<int>();
        inf.InConnections = new List<int>();
        inf.SourceNode = false;
        
        return inf;
    }

    private void RandomNodes()
    {
        var generateNodes = Nodes.Where(c => c.SourceNode == true).ToList();

        int steps = Random.Range(5, 20);
        float dist = Random.Range(10, 30);

        for (int i = 0; i < steps; i++)
        {
            List<NodeInf> newInfos = new List<NodeInf>();

            foreach (var node in generateNodes)
            {
                var newNode1 = GenerateNewNode(node, dist);

                if (newNode1 == null)
                    continue;

                NodeInf newNode2 = null;

                if (Random.Range(0, 4) == 0)
                    newNode2 = GenerateNewNode(node, dist);

                if (newNode2 != null && Vector2.Distance(newNode1.Position, newNode2.Position) < 1)
                    newNode2 = null;

                newNode1.InConnections.Add(node.ID);
                node.OutConnections.Add(newNode1.ID);
                Nodes.Add(newNode1);
                newInfos.Add(newNode1);
                
                if (newNode2 != null)
                {
                    newNode2.InConnections.Add(node.ID);
                    node.OutConnections.Add(newNode2.ID);
                    Nodes.Add(newNode2);
                    newInfos.Add(newNode2);
                }
            }

            generateNodes = newInfos;
            if (generateNodes.Count < 1)
                break;
        }
    }

    public class NodeInf
    {
        public Vector2 Position { get; set; }
        public int ID { get; set; }
        public List<int> OutConnections { get; set; }
        public List<int> InConnections { get; set; }
        public bool SourceNode { get; set; }

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

            clone.SourceNode = SourceNode;

            return clone;
        }
    }
}