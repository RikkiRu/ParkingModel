using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    private GameObject pointer;
    public GameObject Poiner { get { return pointer; } }
    public event Action PointerPositionChanged = delegate { };
    private Vector3 LastCursorPos { get; set; }

    private void Awake()
    {
        LastCursorPos = Vector3.zero;
        pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointer.GetComponent<MeshRenderer>().material.color = Color.red;
        pointer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        pointer.transform.Rotate(Vector3.right, 90f);
        MapCreatorLoader.Instance.Attach(pointer);
    }

    private void Update()
    {
        var translationZ = Input.GetAxis("Vertical") * moveSpeed;
        var translationX = Input.GetAxis("Horizontal") * moveSpeed;

        translationZ *= Time.deltaTime;
        translationX *= Time.deltaTime;

        transform.Translate(translationX, 0, translationZ);
        //transform.Rotate(0, translationY, 0);

        if (Input.GetMouseButton(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            MapCreatorLoader.Instance.GroundPlane.Raycast(ray, out dist);
            var point = ray.GetPoint(dist);
            point.y = 0f;

            if (LastCursorPos == point)
                return;
            LastCursorPos = point;

            pointer.transform.position = point;
            PointerPositionChanged();

            bool l = MapCreatorLoader.Instance.ParkingZone.CanPlaceTo(GeometryUtil.V2(point));
            if (l)
                pointer.GetComponent<MeshRenderer>().material.color = Color.green;
            else
                pointer.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
