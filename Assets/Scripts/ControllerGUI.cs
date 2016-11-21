using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGUI : MonoBehaviour
{
    [SerializeField] Button quadZone;
    [SerializeField] Button pointAdd;
    [SerializeField] Button pointRemove;
    [SerializeField] Button pointEdit;
    [SerializeField] Button makeNode;
    [SerializeField] Button makeConnectedNode;
    [SerializeField] Button removeNode;
    [SerializeField] Text pointEditText;
    [SerializeField] QuadZone quadZonePrefab;

    private bool pointEditEnabled;

    private void Awake()
    {
        quadZone.onClick.AddListener(OnQuadZoneClick);
        pointAdd.onClick.AddListener(OnPointAddClick);
        pointRemove.onClick.AddListener(OnPointRemoveClick);
        pointEdit.onClick.AddListener(OnPointEditClick);
        makeNode.onClick.AddListener(OnMakeNodeClick);
        makeConnectedNode.onClick.AddListener(OnMakeConnectedNode);
        removeNode.onClick.AddListener(OnRemoveNodeClick);
    }

    private void OnMakeConnectedNode()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.AddNode(true);
    }

    private void OnRemoveNodeClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.RemoveNode();
    }

    private void OnMakeNodeClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.AddNode(false);
    }

    private void OnPointEditClick()
    {
        pointEditEnabled = !pointEditEnabled;
        CheckPointEdit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnPointEditClick();
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
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.RemovePoint();
    }

    private void OnPointAddClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.AddPoint();
    }

    private void CommonClick()
    {
        pointEditEnabled = false;
        CheckPointEdit();
    }

    private void OnQuadZoneClick()
    {
        CommonClick();

        QuadZone zone = Instantiate(quadZonePrefab);
        MapCreatorLoader.Instance.Attach(zone.gameObject);
        zone.transform.localPosition = 
            MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition + 
            new Vector3(0, 0.2f, 0);
    }
}
