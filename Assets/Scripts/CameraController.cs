using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    private void Update ()
    {
        var translationZ = Input.GetAxis("Vertical") * moveSpeed;
        var translationX = Input.GetAxis("Horizontal") * moveSpeed;

        translationZ *= Time.deltaTime;
        translationX *= Time.deltaTime;

        transform.Translate(translationX, 0, translationZ);
        //transform.Rotate(0, translationY, 0);
    }
}
