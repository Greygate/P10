using UnityEngine;
using TelloLib;

public class Drone : MonoBehaviour
{
    public float droneSpeed = 5f;

    [Range(-1, 1)]
    public float lx = 0f;
    [Range(-1, 1)]
    public float ly = 0f;
    [Range(-1, 1)]
    public float rx = 0f;
    [Range(-1, 1)]
    public float ry = 0f;
    [Range(1, 5)]
    public int height = 1;


    Vector3 startPosition;

    bool alerted = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        //Subscribe to Tello connection events. Called when connected/disconnected.
        Tello.onConnection += (Tello.ConnectionState newState) =>
        {
            //Show connection messages.
            Debug.Log($"Tello connection: {newState.ToString()}");
        };

        //subscribe to Tello update events. Called when update data arrives from drone.
        Tello.onUpdate += (int newState) =>
        {
            Debug.Log(newState);
        };

        Tello.StartConnecting();//Start trying to connect.
    }

    // Update is called once per frame
    void Update()
    {
        if (alerted)
        {
            // Make path from start position to current position
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log($"Old state: {Tello.state.ToString()}");
            if (Tello.state.flying)
            {
                Debug.Log("Landing");
                Tello.Land();
            }
            else
            {
                Debug.Log("Taking off");
                Tello.TakeOff();
            }
            Debug.Log($"New state: {Tello.state.ToString()}");
        }

        if (Tello.connectionState == Tello.ConnectionState.Connected)
        {
            Tello.controllerState.SetAxis(lx, ly, rx, ry);
            Tello.SetMaxHeight(height);
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }
}
