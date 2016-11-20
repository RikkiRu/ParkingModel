using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGUI : MonoBehaviour
{
    [SerializeField] Button quadZone;
    [SerializeField] Button pointAdd;
    [SerializeField] Button pointRemove;
    [SerializeField] Button pointEdit;

    [SerializeField] Text pointEditText;

    [SerializeField] QuadZone quadZonePrefab;

    private bool pointEditEnabled;

    private void Awake()
    {
        quadZone.onClick.AddListener(OnQuadZoneClick);
        pointAdd.onClick.AddListener(OnPointAddClick);
        pointRemove.onClick.AddListener(OnPointRemoveClick);
        pointEdit.onClick.AddListener(OnPointEditClick);
    }

    private void OnPointEditClick()
    {
        pointEditEnabled = !pointEditEnabled;
        CheckPointEdit();
    }

    private void CheckPointEdit()
    {
        if (pointEditEnabled)
            pointEditText.text = "Edit enabled";
        else
            pointEditText.text = "Edit disabled";

        MapCreatorLoader.Instance.ParkingZone.ToggleEdit(pointEditEnabled);
    }

    private void OnPointRemoveClick()
    {
        pointEditEnabled = false;
        CheckPointEdit();

        MapCreatorLoader.Instance.ParkingZone.AddPoint();
    }

    private void OnPointAddClick()
    {
        pointEditEnabled = false;
        CheckPointEdit();

        MapCreatorLoader.Instance.ParkingZone.RemovePoint();
    }

    private void OnQuadZoneClick()
    {
        pointEditEnabled = false;
        CheckPointEdit();

        QuadZone zone = Instantiate(quadZonePrefab);
        MapCreatorLoader.Instance.Attach(zone.gameObject);
        zone.transform.localPosition = 
            MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition + 
            new Vector3(0, 0.2f, 0);
    }
}
