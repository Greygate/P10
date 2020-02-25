using UnityEngine;
using UnityVicon;

[RequireComponent(typeof(ViconDataStreamClient))]
public class ViconManager : MonoBehaviour
{
    public int numDrones = 1;

    public GameObject dronePrefab;

    ViconDataStreamClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<ViconDataStreamClient>();

        for (int i = 0; i < numDrones; i++)
        {
            GameObject drone = Instantiate(dronePrefab);
            RBScript script = drone.AddComponent<RBScript>();
            script.Client = client;
            script.ObjectName = $"Drone_{i}";
        }
    }
}
