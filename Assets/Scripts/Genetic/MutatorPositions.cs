using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutatorPositions : IMutator
{
    public static MutatorPositions Instance = new MutatorPositions();

    public void ApplyTo(GeneticEssence essence)
    {
        var generatedNodes = essence.Nodes.Where(c => c.SourceNode == false).ToList();

        if (generatedNodes.Count < 1)
            return;

        GeneticEssence.NodeInf target = generatedNodes[Random.Range(0, generatedNodes.Count)];

        float offset = 5f;
        float rx = Random.Range(-offset, offset);
        float ry = Random.Range(-offset, offset);

        target.Position += new Vector2(rx, ry);

        List<int> toRemove = new List<int>();

        foreach (var id in target.OutConnections)
        {
            var node = generatedNodes.FirstOrDefault(c => c.ID == id);

            if (node == default(GeneticEssence.NodeInf))
            {
                toRemove.Add(id);
                continue;
            }

            if (Vector2.Distance(node.Position, target.Position) < 1)
                toRemove.Add(node.ID);
        }

        foreach (var id in toRemove)
            MutatorRemover.Instance.Remove(id, essence);
    }
}