using System;
using UnityEngine;

public class MapCreatorLoader : MonoBehaviour
{
    public static MapCreatorLoader Instance;

    public static Vector2 Pointer2d
    {
        get
        {
            Vector3 p = Instance.CameraInstance.Poiner.transform.localPosition;
            return new Vector2(p.x, p.z);
        }
    }

    public static Vector3 Pointer3d
    {
        get
        {
            Vector3 p = Instance.CameraInstance.Poiner.transform.localPosition;
            return new Vector3(p.x, p.y, p.z);
        }
    }

    [SerializeField] CameraController cameraPrefab;
    [SerializeField] GameObject baseMapObjectPrefab;
    [SerializeField] ControllerGUI guiControllerPrefab;
    [SerializeField] ParkingZone parkingZonePrefab;
    [SerializeField] GeneticController geneticControllerPrefab;

    private const float sizeX = 10000;
    private const float sizeY = sizeX;
    private static readonly Color32 color = new Color32(140, 180, 70, 255);

    private CameraController cameraInstance;
    private GameObject baseMapObject;
    private GameObject ground;
    private Plane groundPlane;
    private ControllerGUI guiController;
    private ParkingZone parkingZone;
    private GeneticController geneticController;

    public CameraController CameraInstance { get { return cameraInstance; } }
    public GameObject BaseMapObject { get { return baseMapObject; } }
    public GameObject Ground { get { return ground; } }
    public Plane GroundPlane { get { return groundPlane; } }
    public ParkingZone ParkingZone { get { return parkingZone; } }
    public GeneticController GeneticController { get { return geneticController; } }

    public void Init()
    {
        Instance = this;

        MakeBaseMapObject();
        MakeGround();
        MakeCamera();
        MakeGui();
        MakeParkingZone();
        MakeGenetic();
    }

    private void MakeGenetic()
    {
        geneticController = Instantiate(geneticControllerPrefab);
        Attach(geneticController.gameObject);
    }

    private void MakeParkingZone()
    {
        parkingZone = Instantiate(parkingZonePrefab);
        Attach(parkingZone.gameObject);
    }

    private void MakeBaseMapObject()
    {
        baseMapObject = Instantiate(baseMapObjectPrefab);
    }

    private void MakeCamera()
    {
        cameraInstance = Instantiate(cameraPrefab);
        cameraInstance.transform.SetParent(baseMapObject.transform, false);
    }

    private void MakeGround()
    {
        ground = MeshUtil.MakeSolidMesh("Ground", sizeX, sizeY, color);
        ground.transform.SetParent(baseMapObject.transform, false);
        MeshRenderer render = ground.GetComponent<MeshRenderer>();
        render.sortingOrder = SortingOrder.Is(Layer.Ground);

        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void MakeGui()
    {
        guiController = Instantiate(guiControllerPrefab);
        guiController.transform.SetParent(baseMapObject.transform, false);
    }

    public void Attach(GameObject obj)
    {
        obj.transform.SetParent(baseMapObject.transform, false);
    }
}