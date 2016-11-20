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
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void Start()
    {
        ReDraw();
    }

    public void AddPoint()
    {
    }

    public void RemovePoint()
    {
    }

    private void FindEditVertex()
    {
        int indxMinDist = -1;
        float minDist = float.MaxValue;

        Vector3 pointer = MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition;
        Vector2 pointer2d = new Vector2(pointer.x, pointer.z);

        for (int i = 0; i < vertices2D.Count; i++)
        {
            Vector2 check = vertices2D[i];
            float dist = Vector2.Distance(check, pointer2d);

            if (dist < minDist)
            {
                indxMinDist = i;
                minDist = dist;
            }
        }

        editVertexIndx = indxMinDist;
    }

    public void ToggleEdit(bool active)
    {
        if (active)
            FindEditVertex();

        editActive = active;
    }
}