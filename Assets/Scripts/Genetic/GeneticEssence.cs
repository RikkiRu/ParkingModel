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

    private NodeInf AddNewNode()
    {
        NodeInf inf = new NodeInf();
        float x = Random.Range(Controller.BoundsMin.x, Controller.BoundsMax.x);
        float y = Random.Range(Controller.BoundsMin.y, Controller.BoundsMax.y);

        while (!MapCreatorLoader.Instance.ParkingZone.CanPlaceTo(new Vector2(x, y)))
        {
            x = Random.Range(Controller.BoundsMin.x, Controller.BoundsMax.x);
            y = Random.Range(Controller.BoundsMin.y, Controller.BoundsMax.y);
        }

        inf.Position = new Vector2(x, y);
        inf.ID = IdCounter;
        IdCounter++;
        inf.OutConnections = new List<int>();
        inf.InConnections = new List<int>();
        inf.SourceNode = false;
        Nodes.Add(inf);
        return inf;
    }

    private void RandomNodes()
    {
        var generateNodes = Nodes.Where(c => c.SourceNode == true).ToList();

        const int Steps = 4;

        for (int i = 0; i < Steps; i++)
        {
            List<NodeInf> newInfos = new List<NodeInf>();

            foreach (var node in generateNodes)
            {
                // Generate only correct angeles
                var newNode = AddNewNode();
                newNode.InConnections.Add(node.ID);
                node.OutConnections.Add(newNode.ID);
                newInfos.Add(newNode);
            }

            generateNodes = newInfos;
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