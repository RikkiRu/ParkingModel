using System.Collections.Generic;
using UnityEngine;

public class ParkingPlace : MonoBehaviour
{
    [SerializeField] QuadZone quadZonePrefab;

    public const float NormSize = 4.2f;
    public const float DirSize = 2.34f;
    public const float SplitDist = 0.2f;

    private QuadZone zone;
    private int ShapeID { get; set; }

    public static List<Vector2> GetVertices(Vector2 origin, Vector2 direction)
    {
        Vector2 dirOrigin = new Vector2(origin.x + direction.x * NormSize / 2, origin.y + direction.y * NormSize / 2);

        GeometryUtil.LineOptions lineOpt = new GeometryUtil.LineOptions(origin, dirOrigin);
        lineOpt.CalcNormScale(DirSize / 2);

        Vector2 q1 = lineOpt.MakeNormalOffset(origin, -1);
        Vector2 q2 = lineOpt.MakeNormalOffset(origin, +1);
        Vector2 q3 = lineOpt.MakeNormalOffset(dirOrigin, -1);
        Vector2 q4 = lineOpt.MakeNormalOffset(dirOrigin, +1);

        List<Vector2> v = new List<Vector2>();
        v.Add(q1);
        v.Add(q2);
        v.Add(q3);
        v.Add(q4);

        return v;
    }

    public void SpawnShape(List<Vector2> v, bool can)
    {
        var quad = Instantiate(quadZonePrefab);
        MapCreatorLoader.Instance.Attach(quad.gameObject);

        if (can)
            quad.Color = Colors.PlaceColor;
        else
            quad.Color = Colors.CantPlace;

        quad.Init(v[0], v[1], v[2], v[3]);
        var quadRender = quad.gameObject.GetComponent<MeshRenderer>();

        int sorting = can ? SortingOrder.Is(Layer.Place) : SortingOrder.Is(Layer.WrongPlace);

        quadRender.sortingOrder = sorting;
        zone = quad;

        ShapeID = MapCreatorLoader.Instance.ParkingZone.AddShape(v, false);
        quad.name = "Place_shape_" + ShapeID;
    }

    public void RemoveShapeNow()
    {
        MapCreatorLoader.Instance.ParkingZone.RemoveShape(ShapeID);
    }

    private void OnDestroy()
    {
        if (zone != null)
            Destroy(zone.gameObject);
    }
}