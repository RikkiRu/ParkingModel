using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGUI : MonoBehaviour
{
    [SerializeField] Button pointAdd;
    [SerializeField] Button pointRemove;
    [SerializeField] Button pointEdit;
    [SerializeField] Button makeNode;
    [SerializeField] Button makeConnectedNode;
    [SerializeField] Button removeNode;
    [SerializeField] Button makePlaces;
    [SerializeField] Button geneticStart;
    [SerializeField] Button geneticStop;

    [SerializeField] Text pointEditText;

    private bool pointEditEnabled;

    private void Awake()
    {
        pointAdd.onClick.AddListener(OnPointAddClick);
        pointRemove.onClick.AddListener(OnPointRemoveClick);
        pointEdit.onClick.AddListener(OnPointEditClick);
        makeNode.onClick.AddListener(OnMakeNodeClick);
        makeConnectedNode.onClick.AddListener(OnMakeConnectedNode);
        removeNode.onClick.AddListener(OnRemoveNodeClick);
        makePlaces.onClick.AddListener(OnMakePlacesClick);
        geneticStart.onClick.AddListener(OnGeneticStart);
        geneticStop.onClick.AddListener(OnGeneticStop);
    }

    private void OnGeneticStop()
    {
        MapCreatorLoader.Instance.GeneticController.SetActive(false);
    }

    private void OnGeneticStart()
    {
        MapCreatorLoader.Instance.GeneticController.SetActive(true);
    }

    private void OnMakePlacesClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.MakePlaces();
    }

    private void OnMakeConnectedNode()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.ConnectNodesMode();
    }

    private void OnRemoveNodeClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.RemoveNode();
    }

    private void OnMakeNodeClick()
    {
        CommonClick();
        MapCreatorLoader.Instance.ParkingZone.AddNode();
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
            pointEditText.text = "Edit V ON";
        else
            pointEditText.text = "Edit V OFF";

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
}