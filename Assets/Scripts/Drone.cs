using UnityEngine;
using TelloLib;

public class Drone : MonoBehaviour
{
    [Range(1, 100)]
    public float droneSpeed = 50;

    [Range(1, 10)]
    public int height = 1;

    bool alerted = false;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to Tello connection events. Called when connected/disconnected.
        Tello.onConnection += (ConnectionState newState) =>
        {
            //Show connection messages.
            Debug.Log($"Tello connection: {newState.ToString()}");
        };

        Tello.onUpdate += (int newState) =>
        {
            Debug.Log($"Current Loc: x:{Tello.state.posX}, y:{Tello.state.posY}, z:{Tello.state.posZ}, remaining fly time:{Tello.state.droneFlyTimeLeft}");
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
            if (Tello.state.flying)
            {
                Debug.Log("Landing");
                Tello.Land();
            }
            else
            {
                if (Tello.state.batteryLower)
                {
                    Debug.LogError("Battery too low for take off");
                    return;
                }

                if (Tello.state.batteryLow)
                {
                    Debug.LogWarning("Battery low");
                }

                Debug.Log("Taking off");
                Tello.TakeOff();
            }
        }

        if (Tello.ConnectionState == ConnectionState.Connected)
        {
            Tello.controllerState.SetAxis(Input.GetAxis("Rotate") * droneSpeed / 100, Input.GetAxis("Levitate") * droneSpeed / 100, Input.GetAxis("LRTranslate") * droneSpeed / 100, Input.GetAxis("FBTranslate") * droneSpeed / 100);
            Tello.SetMaxHeight(height);
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }
}
