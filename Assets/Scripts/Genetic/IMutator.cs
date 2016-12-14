using UnityEngine;
using System.Collections;

public interface IMutator
{
    void ApplyTo(GeneticEssence essence);
}