using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    private GameObject pointer;

    private void Awake()
    {
        pointer = MeshUtil.MakeSolidMesh("Pointer", 1, 1, Color.red);
        pointer.transform.Rotate(Vector3.right, 90f);
        pointer.transform.SetParent(MapCreatorLoader.Instance.BaseMapObject.transform);
    }

    private void Update ()
    {
        var translationZ = Input.GetAxis("Vertical") * moveSpeed;
        var translationX = Input.GetAxis("Horizontal") * moveSpeed;

        translationZ *= Time.deltaTime;
        translationX *= Time.deltaTime;

        transform.Translate(translationX, 0, translationZ);
        //transform.Rotate(0, translationY, 0);

        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //var point = ray.origin + (ray.direction * 5);
            
            //point.y = 0.1f;
            //pointer.transform.position = point;

            float dist;
            MapCreatorLoader.Instance.GroundPlane.Raycast(ray, out dist);
            var point = ray.GetPoint(dist);

            Debug.Log("World point " + point);
            point.y = 0.1f;
            pointer.transform.position = point;
        }
    }
}
