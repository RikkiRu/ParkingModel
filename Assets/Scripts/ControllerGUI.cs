using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ControllerGUI : MonoBehaviour
{
    [SerializeField] Button quadZone;
    [SerializeField] QuadZone quadZonePrefab;

    private void Awake()
    {
        quadZone.onClick.AddListener(OnQuadZoneClick);
    }

    private void OnQuadZoneClick()
    {
        QuadZone zone = Instantiate(quadZonePrefab);
        zone.transform.SetParent(MapCreatorLoader.Instance.BaseMapObject.transform, false);
        zone.transform.localPosition = 
            MapCreatorLoader.Instance.CameraInstance.Poiner.transform.localPosition + 
            new Vector3(0, 0.2f, 0);
    }
}
