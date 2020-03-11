using UnityEngine;
using TelloLib;
using OpenCvSharp;
using System.Collections;

[RequireComponent(typeof(Navigation))]
[RequireComponent(typeof(PathVisualizer))]
public class Drone : MonoBehaviour
{
    public bool debug = false;
    [SerializeField]
    Transform targetPos;

    bool alerted = false;
    bool hasPath = false;

    Navigation navigation;
    PathVisualizer pathVisualizer;

    void Awake()
    {
        navigation = GetComponent<Navigation>();
        pathVisualizer = GetComponent<PathVisualizer>();

        Tello.onConnection += (ConnectionState state) =>
        {
            Debug.Log($"Tello Connection: {state}");
            switch (state)
            {
                case ConnectionState.Disconnected:
                case ConnectionState.Paused:
                    //StopCoroutine(ScanForHumans());
                    break;
                case ConnectionState.Connected:

                    //Tello.SetPicVidMode(0);
                    //StartCoroutine(ScanForHumans());
                    //navigation.InitializeNavigation();
                    break;
                case ConnectionState.UnPausing:
                    //StartCoroutine(ScanForHumans());
                    break;
                default:
                    break;
            }
        };

        Tello.onUpdate += (int newState) =>
        {
            Debug.Log($"UPDATE: New state {newState}");

            if (newState == 100)
            {
                //Image is done
                if (DetectHuman(new Mat(Tello.picFilePath)))
                {
                    alerted = true;
                    Debug.Log("No human found");
                }
                else
                {
                    Debug.Log("No human found");
                }
            }
        };

        Tello.StartConnecting();
    }

    // Update is called once per frame
    void Update()
    {
        if (Tello.Connected)
        {
            transform.position = navigation.GetCurrentPosition();
            targetPos.localPosition = navigation.GetWantedPosition();
        }
        
        if (alerted)
        {
            if (!hasPath)
            {
                pathVisualizer.CreatePath(new Vector3[] { navigation.GetStartPosition(), targetPos.position });
                hasPath = true;
            }
        }
        else
        {
            if (hasPath)
            {
                pathVisualizer.ResetPath();
                hasPath = false;
            }
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
                else if (Tello.state.batteryLow)
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
            if (debug || Tello.state.flying)
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
            if (debug || Tello.state.flying)
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
            if (debug || Tello.state.flying)
            {
                Debug.Log("Drone: Moving forwards");
                navigation.MoveForwards();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (debug || Tello.state.flying)
            {
                Debug.Log("Moving backwards");
                navigation.MoveBackwards();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            alerted = !alerted;
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }

    IEnumerator ScanForHumans()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Tello.TakePicture();
        }
    }

    public bool DetectHuman(Mat image)
    {
        OpenCvSharp.Rect[] regions;

        using (HOGDescriptor des = new HOGDescriptor())
        {
            des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

            regions = des.DetectMultiScale(image);
        }

        return regions.Length > 0;
    }
}
