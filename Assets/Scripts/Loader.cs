using UnityEngine;
using UnityEngine.EventSystems;

public class Loader : MonoBehaviour
{
    [SerializeField] MapCreatorLoader mapCreatorPrefab;
    [SerializeField] EventSystem eventSystemPrefab;

    private MapCreatorLoader mapCreator;
    private EventSystem eventSystem;

    private void Awake()
    {
        mapCreator = Instantiate(mapCreatorPrefab);
        mapCreator.Init();

        eventSystem = Instantiate(eventSystemPrefab);
    }
}
