using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticController : MonoBehaviour
{
    public static GeneticController Instance { get; set; }

    private IEnumerator GeneticCoroutine { get; set; }
    public Vector2 BoundsMin { get; set; }
    public Vector2 BoundsMax { get; set; }
    private List<GeneticEssence> Essences { get; set; }

    private ParkingZone Zone
    {
        get { return MapCreatorLoader.Instance.ParkingZone; }
    }

    private void Awake()
    {
        GeneticCoroutine = null;
        Instance = this;
    }

    private void Clear()
    {
        Zone.RemoveNonUserNodes();
        Essences = new List<GeneticEssence>();
    }

    private void OnDestroy()
    {
        Clear();
        Instance = null;
    }

    public void SetActive(bool isOn)
    {
        bool haveCoroutine = GeneticCoroutine != null;

        if (haveCoroutine == isOn)
            return;

        if (isOn)
        {
            Clear();
            GeneticCoroutine = GeneticProcess();
            StartCoroutine(GeneticCoroutine);
        }
        else
        {
            StopCoroutine(GeneticCoroutine);
            GeneticCoroutine = null;
            Clear();
        }
    }

    private void CalcBounds()
    {
        var info = Zone.GetZoneShape();

        float minX = info.V.Min(c => c.x);
        float maxX = info.V.Max(c => c.x);
        float minY = info.V.Min(c => c.y);
        float maxY = info.V.Max(c => c.y);

        BoundsMin = new Vector2(minX, minY);
        BoundsMax = new Vector2(maxX, maxY);
    }

    public PathNode MakeNode(Vector2 position)
    {
        return Zone.AddProgramNode(GeometryUtil.V3(position), false);
    }

    private IEnumerator GeneticProcess()
    {
        CalcBounds();

        while (true)
        {
            yield return null;
        }
    }

    private int FitnessFunction(GeneticEssence essence)
    {
        return 0;
    }
}