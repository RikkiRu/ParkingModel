using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutatorRemover : IMutator
{
    public static MutatorRemover Instance = new MutatorRemover();

    public void ApplyTo(GeneticEssence essence)
    {
        
    }

    public void Remove(int id, GeneticEssence essence)
    {
        GeneticEssence.NodeInf inf = essence.Nodes.FirstOrDefault(c => c.SourceNode == false && c.ID == id);

        List<GeneticEssence.NodeInf> toCheck = new List<GeneticEssence.NodeInf>();
        toCheck.Add(inf);

        while (toCheck.Count > 0)
        {
            List<GeneticEssence.NodeInf> newCheck = new List<GeneticEssence.NodeInf>();

            foreach (var checking in toCheck)
            {
                foreach (var outId in checking.OutConnections)
                    newCheck.Add(essence.Nodes.FirstOrDefault(c => c.SourceNode == false && c.ID == outId));

                foreach (var c in essence.Nodes)
                {
                    if (c.OutConnections.Contains(checking.ID))
                        c.OutConnections.Remove(checking.ID);
                }

                essence.Nodes.Remove(checking);
            }

            toCheck = newCheck;
        }
    }
}