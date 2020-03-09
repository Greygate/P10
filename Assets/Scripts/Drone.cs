using UnityEngine;
using TelloLib;

[RequireComponent(typeof(Navigation))]
public class Drone : MonoBehaviour
{
    bool alerted = false;

    Navigation navigation;

    // Start is called before the first frame update
    void Start()
    {
        /*
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
        */

        //Start trying to connect.
        navigation = GetComponent<Navigation>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = navigation.GetCurrentPosition();

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

        // DELETE EVERYTHING BETWEEN ME
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(1, 0, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 1, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 1, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 0, 1);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Tello.state.flying)
            {
                Tello.controllerState.SetAxis(0, 0, 0, 0);
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        // AND ME

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Tello.state.flying)
            {
                Debug.Log("Rotating left");
                navigation.MoveLeft();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Tello.state.flying)
            {
                Debug.Log("Rotating right");
                navigation.MoveRight();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Tello.state.flying)
            {
                Debug.Log("Moving forwards");
                navigation.MoveForwards();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Tello.state.flying)
            {
                Debug.Log("Moving backwards");
                navigation.MoveBackwards();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }
}
