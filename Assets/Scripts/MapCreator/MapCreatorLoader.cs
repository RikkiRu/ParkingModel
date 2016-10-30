using UnityEngine;

public class MapCreatorLoader : MonoBehaviour
{
    [SerializeField] CameraController cameraPrefab;

    private const float sizeX = 10000;
    private const float sizeY = sizeX;
    private static readonly Color32 color = new Color32(140, 180, 70, 255);

    private CameraController cameraInstance;

    public void Init()
    {
        MakeCamera();
        MakeGround();
    }

    private void MakeCamera()
    {
        cameraInstance = Instantiate(cameraPrefab);
    }

    private void MakeGround()
    {
        GameObject plane = MeshUtil.MakeSolidMesh("Ground", sizeX, sizeY, color);
        plane.transform.Rotate(Vector3.right, 90f);
    }
}