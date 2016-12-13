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

    private int BestCount = -1;
    private GeneticEssence BestEssence = null;

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

            if (BestEssence != null)
            {
                SpawnFunction(BestEssence);
            }
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

        var userNodes = Zone.GetUserNodes();

        while (true)
        {
            Essences.Clear();
            const int StartSetCount = 1;
            for (int i = 0; i < StartSetCount; i++)
            {
                Essences.Add(new GeneticEssence(userNodes));
            }

            foreach (var i in Essences)
            {
                Clear();
                SpawnFunction(i);
                Zone.MakePlaces();
                int places = Zone.GetPlacesCount();
                //Debug.Log("Places: " + places);

                if (places > BestCount)
                {
                    BestCount = places;
                    BestEssence = i;
                    Debug.Log("Best: " + BestCount);
                }
            }

            yield return null;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private class InfObj
    {
        public GeneticEssence.NodeInf Info { get; set; }
        public PathNode Obj { get; set; }
    }

    private void SpawnFunction(GeneticEssence essence)
    {
        Dictionary<int, InfObj> keys = new Dictionary<int, InfObj>();

        foreach (var i in essence.Nodes)
        {
            var node = MakeNode(i.Position);
            InfObj inf = new InfObj();
            inf.Info = i;
            inf.Obj = node;
            keys.Add(i.ID, inf);
        }

        foreach (var i in essence.Nodes)
        {
            var from = keys[i.ID];

            foreach (var id in i.OutConnections)
            {
                var to = keys[id];
                from.Obj.AddNode(to.Obj);
            }
        }
    }
}