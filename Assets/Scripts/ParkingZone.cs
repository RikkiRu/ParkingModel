using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ParkingZone : MonoBehaviour
{
    private List<Vector2> vertices2D;
    private Mesh mesh;
    private MeshFilter filter;
    private bool editActive;
    private int editVertexIndx;
    private Transform sphereHolder;
    private List<GameObject> spheres;

    private void Awake()
    {
        vertices2D = new List<Vector2>();
        vertices2D.Add(new Vector2(0, 0));
        vertices2D.Add(new Vector2(0, 25));
        vertices2D.Add(new Vector2(25, 25));
        vertices2D.Add(new Vector2(25, 50));
        vertices2D.Add(new Vector2(0, 50));
        vertices2D.Add(new Vector2(0, 75));
        vertices2D.Add(new Vector2(75, 75));
        vertices2D.Add(new Vector2(75, 50));
        vertices2D.Add(new Vector2(50, 50));
        vertices2D.Add(new Vector2(50, 25));
        vertices2D.Add(new Vector2(75, 25));
        vertices2D.Add(new Vector2(75, 0));

        mesh = new Mesh();
        mesh.name = "parkingZoneMesh";
        filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        MeshUtil.ApplyMaterial(gameObject, new Color32(165, 165, 165, 255));

        transform.Rotate(Vector3.right, 90f);
        transform.localPosition = new Vector3(0, 0.1f, 0);

        spheres = new List<GameObject>();
        GameObject sphereHolderObj = new GameObject("SphereHolder");
        sphereHolderObj.transform.SetParent(transform, false);
        sphereHolder = sphereHolderObj.transform;
    }

    private void OnEnable()
    {
        MapCreatorLoader.Instance.CameraInstance.PointerPositionChanged += PointerPositionChanged;
    }

    private void OnDisable()
    {
        MapCreatorLoader.Instance.CameraInstance.PointerPositionChanged -= PointerPositionChanged;
    }

    private void PointerPositionChanged()
    {
        if (editActive)
        {
            if (vertices2D.Count <= editVertexIndx || editVertexIndx < 0)
            {
                Debug.LogWarning("Invalid point", this);
                return;
            }

            var pos = MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition;
            vertices2D[editVertexIndx] = new Vector2(pos.x, pos.z);
            ReDraw();
        }
    }

    private void ReDraw()
    {
        int[] indices = Triangulator.Triangulate(vertices2D);
        Vector3[] vertices = vertices2D.Select(c => new Vector3(c.x, c.y, 0)).ToArray();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        foreach (var i in spheres)
            Destroy(i.gameObject);

        spheres.Clear();

        foreach (var i in vertices2D)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(sphereHolder, false);
            sphere.transform.localPosition = new Vector3(i.x, i.y, 0);
            sphere.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            sphere.GetComponent<MeshRenderer>().material.color = Color.blue;
            spheres.Add(sphere);
        }
    }

    private void Start()
    {
        ReDraw();
    }

    public void AddPoint()
    {
        List<Vector2> averages = new List<Vector2>();

        for (int i = 0; i < vertices2D.Count - 1; i++)
        {
            Vector2 v1 = vertices2D[i];
            Vector2 v2 = vertices2D[i + 1];
            Vector2 n = new Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
            averages.Add(n);
        }

        Vector2 vn1 = vertices2D[0];
        Vector2 vn2 = vertices2D[vertices2D.Count - 1];
        Vector2 vn = new Vector2((vn1.x + vn2.x) / 2, (vn1.y + vn2.y) / 2);
        averages.Add(vn);

        Vector3 p = MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition;
        Vector2 p2d = new Vector2(p.x, p.z);
        int closest = FindClosest(averages, p2d);

        int insertPoint = closest + 1;
        if (insertPoint >= vertices2D.Count)
            insertPoint = 0;

        vertices2D.Insert(insertPoint, p2d);
        ReDraw();
    }

    public void RemovePoint()
    {
        if(vertices2D.Count <= 4)
        {
            Debug.LogWarning("To small points count");
            return;
        }

        Vector3 p = MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition;
        int indxMinDist = FindClosest(vertices2D, new Vector2(p.x, p.z));
        vertices2D.RemoveAt(indxMinDist);
        ReDraw();
    }

    private int FindClosest(List<Vector2> list, Vector2 p)
    {
        int indxMinDist = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            Vector2 check = list[i];
            float dist = Vector2.Distance(check, p);

            if (dist < minDist)
            {
                indxMinDist = i;
                minDist = dist;
            }
        }

        return indxMinDist;
    }

    private void FindEditVertex()
    {
        Vector3 p = MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition;
        int indxMinDist = FindClosest(vertices2D, new Vector2(p.x, p.z));
        editVertexIndx = indxMinDist;
    }

    public void ToggleEdit(bool active)
    {
        if (active)
            FindEditVertex();

        editActive = active;
    }
}