using UnityEngine;
using TelloLib;

[RequireComponent(typeof(Navigation))]
[RequireComponent(typeof(PathVisualizer))]
public class Drone : MonoBehaviour
{
    public bool debug = false;
    [SerializeField]
    Transform targetPos;

    bool alerted = false;

    Navigation navigation;
    PathVisualizer pathVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        navigation = GetComponent<Navigation>();
        pathVisualizer = GetComponent<PathVisualizer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alerted)
        {
            pathVisualizer.CreatePath(new Vector3[] { navigation.GetStartPosition(), transform.position });
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }
}