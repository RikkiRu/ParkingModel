using UnityEngine;

public class MapCreatorLoader : MonoBehaviour
{
    public static MapCreatorLoader Instance;

    [SerializeField] CameraController cameraPrefab;
    [SerializeField] GameObject baseMapObjectPrefab;

    private const float sizeX = 10000;
    private const float sizeY = sizeX;
    private static readonly Color32 color = new Color32(140, 180, 70, 255);

    private CameraController cameraInstance;
    private GameObject baseMapObject;
    private GameObject ground;
    private Plane groundPlane;

    public GameObject BaseMapObject { get { return baseMapObject; } }
    public GameObject Ground { get { return ground; } }
    public Plane GroundPlane { get { return groundPlane; } }

    public void Init()
    {
        Instance = this;

        MakeBaseMapObject();
        MakeGround();
        MakeCamera();
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
        ground.transform.Rotate(Vector3.right, 90f);

        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }
}