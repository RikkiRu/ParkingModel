using System.Collections;
using UnityEngine;

public class ParkingPlace : MonoBehaviour
{
    [SerializeField] QuadZone quadZonePrefab;

    public const float NormSize = 4.2f;
    public const float DirSize = 2.34f;
    public const float SplitDist = 0.2f;

    private QuadZone zone;

    public void SpawnShape(Vector2 direction)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.z);
        Vector2 dirOrigin = new Vector2(origin.x + direction.x * NormSize / 2, origin.y + direction.y * NormSize / 2);

        GeometryUtil.LineOptions lineOpt = new GeometryUtil.LineOptions(origin, dirOrigin);
        lineOpt.CalcNormScale(DirSize / 2);

        Vector2 q1 = lineOpt.MakeNormalOffset(origin, -1);
        Vector2 q2 = lineOpt.MakeNormalOffset(origin, +1);
        Vector2 q3 = lineOpt.MakeNormalOffset(dirOrigin, -1);
        Vector2 q4 = lineOpt.MakeNormalOffset(dirOrigin, +1);

        var quad = Instantiate(quadZonePrefab);
        MapCreatorLoader.Instance.Attach(quad.gameObject);
        quad.Color = Colors.PlaceColor;
        quad.Init(q1, q2, q3, q4);
        var quadRender = quad.gameObject.GetComponent<MeshRenderer>();
        quadRender.sortingOrder = SortingOrder.Is(Layer.Place);
        zone = quad;
    }

    private void OnDestroy()
    {
        if (zone != null)
            Destroy(zone.gameObject);
    }
}