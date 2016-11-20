using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    private GameObject pointer;
    public GameObject Poiner { get { return pointer; } }
    public event Action PointerPositionChanged = delegate { };

    private void Awake()
    {
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
            pointer.transform.position = point;
            PointerPositionChanged();
        }
    }
}
