﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkingZone : MonoBehaviour
{
    [SerializeField] PathNode pathNodePrefab;
    
    private Mesh mesh;
    private MeshFilter filter;
    private bool editActive;
    private int editVertexIndx;
    private Transform sphereHolder;
    private Transform nodeHolder;
    private int placesCount = 0;

    private List<Vector2> vertices2D;
    private List<GameObject> spheres;
    private List<PathNode> nodes;
    private List<ShapeInfo> shapeInfos;

    private PathNode connectNode1;
    private bool connectNodesModeOn;
    private int selfShapeId = -1;

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

        var meshRender = MeshUtil.ApplyMaterial(gameObject, new Color32(165, 165, 165, 255));
        meshRender.sortingOrder = SortingOrder.Is(Layer.Polygon);

        spheres = new List<GameObject>();
        GameObject sphereHolderObj = new GameObject("SphereHolder");
        sphereHolderObj.transform.SetParent(transform, false);
        sphereHolder = sphereHolderObj.transform;

        nodes = new List<PathNode>();
        GameObject nodeHolderObj = new GameObject("NodeHolder");
        nodeHolderObj.transform.SetParent(transform, false);
        nodeHolder = nodeHolderObj.transform;

        shapeInfos = new List<ShapeInfo>();
        UpdateSelfShape();
    }

    private void OnEnable()
    {
        MapCreatorLoader.Instance.CameraInstance.PointerPositionChanged += PointerPositionChanged;
    }

    private void OnDisable()
    {
        MapCreatorLoader.Instance.CameraInstance.PointerPositionChanged -= PointerPositionChanged;
    }

    private void OnDestroy()
    {
        RemoveShape(selfShapeId);
    }

    public int AddShape(List<Vector2> positions, bool placesInside)
    {
        ShapeInfo info = new ShapeInfo(positions, placesInside);
        shapeInfos.Add(info);
        return info.ID;
    }

    public void RemoveShape(int id)
    {
        if (id <= 0)
            return;

        shapeInfos.RemoveAll(c => c.ID == id);
    }

    public bool CanPlaceTo(Vector2 position)
    {
        foreach (var i in shapeInfos)
        {
            if (!i.CanPlaceTo(position))
                return false;
        }

        return true;
    }

    private void UpdateSelfShape()
    {
        RemoveShape(selfShapeId);
        selfShapeId = AddShape(vertices2D, true);
    }

    public ShapeInfo GetZoneShape()
    {
        return shapeInfos.Where(c => c.ID == selfShapeId).FirstOrDefault();
    }

    private void PointerPositionChanged()
    {
        Vector2 pos2d = MapCreatorLoader.Pointer2d;

        if (connectNodesModeOn)
        {
            float dist;
            int idClosest = GeometryUtil.FindClosest(nodes, pos2d, out dist);

            if (idClosest < 0 || connectNode1 == null)
            {
                ExitNodeConnectionMode();
                return;
            }

            PathNode closest = nodes[idClosest];

            if (closest == connectNode1)
            {
                Debug.LogWarning("Can't connect node to self");
                ExitNodeConnectionMode();
                return;
            }

            connectNode1.AddNode(closest);
            ExitNodeConnectionMode();
            return;
        }

        if (editActive)
        {
            if (vertices2D.Count <= editVertexIndx || editVertexIndx < 0)
            {
                Debug.LogWarning("Invalid point", this);
                return;
            }

            vertices2D[editVertexIndx] = pos2d;
            UpdateSelfShape();
            ReDraw();
        }
    }

    private void ExitNodeConnectionMode()
    {
        connectNode1 = null;
        connectNodesModeOn = false;
    }

    private void ReDraw()
    {
        int[] indices = Triangulator.Triangulate(vertices2D);
        Vector3[] vertices = vertices2D.Select(c => new Vector3(c.x, 0, c.y)).ToArray();
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
            sphere.transform.localPosition = new Vector3(i.x, 0, i.y);
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

        Vector2 p2d = MapCreatorLoader.Pointer2d;
        int closest = GeometryUtil.FindClosest(averages, p2d);

        int insertPoint = closest + 1;
        if (insertPoint >= vertices2D.Count)
            insertPoint = 0;

        UpdateSelfShape();
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

        int indxMinDist = GeometryUtil.FindClosest(vertices2D, MapCreatorLoader.Pointer2d);
        UpdateSelfShape();
        vertices2D.RemoveAt(indxMinDist);
        ReDraw();
    }

    private void FindEditVertex()
    {
        int indxMinDist = GeometryUtil.FindClosest(vertices2D, MapCreatorLoader.Pointer2d);
        editVertexIndx = indxMinDist;
    }

    public void ToggleEdit(bool active)
    {
        if (active)
            FindEditVertex();

        editActive = active;
    }

    public void ConnectNodesMode()
    {
        Vector2 pos2d = MapCreatorLoader.Pointer2d;
        float dist;
        int idClosest = GeometryUtil.FindClosest(nodes, pos2d, out dist);

        if (idClosest < 0)
        {
            ExitNodeConnectionMode();
            return;
        }

        PathNode closest = nodes[idClosest];

        connectNode1 = closest;
        connectNodesModeOn = true;
    }

    public PathNode AddProgramNode(Vector3 position, bool user)
    {
        PathNode node = Instantiate(pathNodePrefab);
        node.transform.SetParent(nodeHolder, false);
        Vector3 p3d = position;
        node.SetPosition(p3d);
        node.UserNode = user;
        nodes.Add(node);
        return node;
    }

    public void AddNode()
    {
        AddProgramNode(MapCreatorLoader.Pointer3d, true);
    }

    public List<PathNode> GetUserNodes()
    {
        return nodes.Where(c => c.UserNode == true).ToList();
    }

    public void RemoveNonUserNodes()
    {
        var nonUserNodes = nodes.Where(c => c.UserNode == false).ToList();
        foreach (var i in nonUserNodes)
            RemoveNode(i);
    }

    public void RemoveNode()
    {
        ExitNodeConnectionMode();

        Vector2 p2d = MapCreatorLoader.Pointer2d;
        int id = GeometryUtil.FindClosest(nodes, p2d);
        if (id < 0)
            return;

        PathNode removingNode = nodes[id];
        RemoveNode(removingNode);
    }

    private void RemoveNode(PathNode removingNode)
    {
        foreach (var i in nodes)
            i.CheckNodeForRemove(removingNode);

        Destroy(removingNode.gameObject);
        nodes.Remove(removingNode);
    }

    public void MakePlaces()
    {
        placesCount = 0;

        foreach (var i in nodes)
            i.MakePlaces();
    }

    public int GetPlacesCount() { return placesCount; }

    public void PlaceAdded()
    {
        placesCount++;
    }

    public class ShapeInfo
    {
        private static int counter = 0;

        public ShapeInfo(List<Vector2> v, bool placesInside)
        {
            V = new Vector2[v.Count];
            for (int i = 0; i < v.Count; i++)
                V[i] = v[i];

            counter++;
            ID = counter;

            PlacesInside = placesInside;
        }

        public Vector2[] V { get; private set; }
        public int ID { get; private set; }
        public bool PlacesInside { get; private set; }

        public bool CanPlaceTo(Vector2 position)
        {
            bool inPoly = GeometryUtil.Pnpoly(V, position);
            return inPoly == PlacesInside;
        }
    }
}