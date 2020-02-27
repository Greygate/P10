using UnityEngine;
using TelloLib;

public class Drone : MonoBehaviour
{
    public float droneSpeed = 5f;

    Vector3 startPosition;

    bool alerted = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        //Subscribe to Tello connection events. Called when connected/disconnected.
        Tello.onConnection += (Tello.ConnectionState newState) =>
        {
            if (newState == Tello.ConnectionState.Connected)
            {
                Debug.Log("Tello connected!");
                //When connected update maxHeight to 5 meters
                Tello.SetMaxHeight(1);
            }
            //Show connection messages.
            Debug.Log($"Tello State: {newState.ToString()}");
        };

        //subscribe to Tello update events. Called when update data arrives from drone.
        Tello.onUpdate += (int newState) =>
        {
            //Debug.Log($"Tello connection state: {Tello.connectionState.ToString()}");
            //Debug.Log($"Tello fly data: {Tello.state.ToString()}");
            //Debug.Log("FlyMode:" + newState.flyMode + " Height:" + newState.height);
            //Debug.Log(newState);
        };

        Debug.Log("Trying to connect to Trello");
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
            Debug.Log($"Old state: {Tello.state.ToString()} (Flying: {Tello.state.flying})");
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
            Debug.Log($"New state: {Tello.state.ToString()} (Flying: {Tello.state.flying})");
        }

        //transform.Translate(Input.GetAxis("Vertical") * Vector3.up * Time.deltaTime * droneSpeed);

        //transform.Translate(Input.GetAxis("ForwardsBack") * Vector3.forward * Time.deltaTime * droneSpeed);

        //transform.Translate(Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * droneSpeed);
    }

    private void OnDisable()
    {
        Tello.StopConnecting();
    }
}
