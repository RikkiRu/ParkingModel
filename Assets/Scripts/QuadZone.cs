using UnityEngine;
using System.Collections;

public class QuadZone : MonoBehaviour
{
    private Vector3[] vertex;
    private MeshFilter filter;
    private GameObject[] spheres;

    private void Awake()
    {
        vertex = new Vector3[]
        {
            new Vector3(-5, -5, 0f),
            new Vector3(5, -5, 0f),
            new Vector3(-5, 5, 0f),
            new Vector3(5, 5, 0f),
        };

        MeshUtil.CreateMesh(gameObject, 10, 10);
        MeshUtil.ApplyMaterial(gameObject, Color.gray);

        transform.Rotate(Vector3.right, 90f);

        spheres = new GameObject[vertex.Length];
        for (int i=0; i<vertex.Length; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i] = sphere;

            sphere.transform.SetParent(transform, false);
            sphere.transform.localPosition = vertex[i];
            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        filter = gameObject.GetComponent<MeshFilter>();
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        filter.mesh.vertices = vertex;

        for (int i = 0; i < vertex.Length; i++)
            spheres[i].transform.localPosition = vertex[i];
    }
}
