using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TelloLib;

public class Drone : MonoBehaviour
{
    [Range(1, 100)]
    public float droneMovementSpeed = 0.5f;
    public float timeToTravel = 0.1f;

    [Range(1, 100)]
    public float droneRotationalSpeed = 10f;

    [Range(0, 10)]
    public int height = 1;

    bool alerted = false;

    // Drone Pathfinding
    bool busy = false;
    bool positionManagerFirstFire = false;
    bool positionManagerShouldBeRunning = true;
    bool hasSchedulerBeenRun = false; // deleteme
    Queue<Func<IEnumerator>> movements = new Queue<Func<IEnumerator>>(); // deleteme
    NavigationalArray navArray;

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
        Tello.StartConnecting();
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Tello.state.flying)
            {
                navArray = new NavigationalArray(GetDronePosition());
                StartCoroutine(DronePositionManager());
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
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
                navArray.MoveLeft();
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
                navArray.MoveRight();
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
                navArray.MoveForward();
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
                navArray.MoveBackwards();
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Tello.state.flying)
            {
                Debug.Log(GetDronePosition());
                positionManagerShouldBeRunning = false;
            }
            else
            {
                Debug.Log("Can't perform any actions when the drone is not flying... duh..");
            }
        }

        if (Tello.ConnectionState == ConnectionState.Connected)
        {
            Tello.SetMaxHeight(height);
        }
    }

    void OnApplicationQuit()
    {
        Tello.StopConnecting();
    }
    
    public IEnumerator MoveDroneForwards()
    {
        if (busy != true)
        {
            busy = true;

            Tello.controllerState.SetAxis(0, 0, 0, droneMovementSpeed);

            yield return new WaitForSeconds(timeToTravel);

            Tello.controllerState.SetAxis(0, 0, 0, 0);

            yield return new WaitForSeconds(1);

            Debug.Log("moved the drone forwards");

            busy = false;
        }
    }

    public IEnumerator MoveDroneBackwards()
    {
        if (busy != true)
        {
            busy = true;

            Tello.controllerState.SetAxis(0, 0, 0, -droneMovementSpeed);

            yield return new WaitForSeconds(timeToTravel);

            Tello.controllerState.SetAxis(0, 0, 0, 0);

            yield return new WaitForSeconds(1);

            Debug.Log("moved the drone backwards");

            busy = false;
        }
    }

    public IEnumerator MoveDroneLeft()
    {
        if (busy != true)
        {
            busy = true;

            Tello.controllerState.SetAxis(0, 0, -droneMovementSpeed, 0);

            yield return new WaitForSeconds(timeToTravel);

            Tello.controllerState.SetAxis(0, 0, 0, 0);

            yield return new WaitForSeconds(1);

            Debug.Log("moved the drone left");

            busy = false;
        }
    }

    public IEnumerator MoveDroneRight()
    {
        if (busy != true)
        {
            busy = true;

            Tello.controllerState.SetAxis(0, 0, droneMovementSpeed, 0);

            yield return new WaitForSeconds(timeToTravel);

            Tello.controllerState.SetAxis(0, 0, 0, 0);

            yield return new WaitForSeconds(1);

            Debug.Log("moved the drone right");

            busy = false;
        }
    }

    public IEnumerator Scheduler()
    {
        if (hasSchedulerBeenRun != true)
        {
            Debug.Log("Scheduler is starting up.");
            yield return new WaitForSeconds(5);
        }

        Debug.Log("Scheduling has started.");

        while (movements.Count > 0)
        {
            if (busy != true)
            {
                StartCoroutine(movements.Dequeue()());
            }

            // Make sure that the scheduler doesn't waste computational power.
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator DronePositionManager()
    {
        if (positionManagerFirstFire != true)
        {
            positionManagerFirstFire = false;

            Debug.Log("Position Manager is starting up.");

            yield return new WaitForSeconds(5);
        }

        Debug.Log("Position managaging has started.");

        while (positionManagerShouldBeRunning)
        {
            Debug.Log("Real life position: " + GetDronePosition());
            Debug.Log("DronePositionManager position: " + navArray.currentPos);

            Direction direction = navArray.GetSuggestedDirection(GetDronePosition());

            switch (direction)
            {
                case Direction.Forwards:
                    StartCoroutine(MoveDroneForwards());
                    break;
                case Direction.Backwards:
                    StartCoroutine(MoveDroneBackwards());
                    break;
                case Direction.Left:
                    StartCoroutine(MoveDroneLeft());
                    break;
                case Direction.Right:
                    StartCoroutine(MoveDroneRight());
                    break;
                case Direction.None:
                    break;
                default:
                    throw new Exception("FUUUUUUUUUUUUUUUUUUUUUUUUUUCK");
                    break;
            }

            yield return new WaitForSeconds(1);

            Debug.Log("________________________");
        }
    }

    public void WallFollowerPathFinding()
    {
        bool ShouldIBeLookingForSurvivor = true;

        // save current position and rotation: POS 0

        Vector3 pos0, pos1 = GetDronePosition();


        while (ShouldIBeLookingForSurvivor)
        {
            // save current position and rotation: POS 1
            // rotate right
            // fly forward

            if (false /* if tracker says that the drone didn't move */)
            {
                // return to: POS 1
                // fly forward

                if (false /* if tracker says that the drone didn't move */)
                {
                    // return to: POS 1
                    // rotate left
                    // fly forward

                    if (false /* if tracker says that the drone didn't move */)
                    {
                        // the room is 1x1 and the drone can therefore not explore the building further.
                    }

                    // check for survivors
                }

                // check for survivors
            }

            // check for survivors
        }
    }

    private Vector3 GetDronePosition()
    {
        return new Vector3(Tello.state.posX, Tello.state.posY, Tello.state.posZ);
    }

    [Serializable]
    public class NavigationalArray
    {
        public Vector3 startPos;
        public Vector3 currentPos;

        public float modifyer = 1f;

        public NavigationalArray(Vector3 startPosition)
        {
            startPos = startPosition;
            currentPos = startPosition;
        }

        public Vector3 GetGridPosition(int x, int y)
        {
            return new Vector3(startPos.x + (x * modifyer), startPos.y, startPos.z + (y * modifyer));
        }

        public void MoveForward()
        {
            currentPos.z += 1;
        }
        public void MoveBackwards()
        {
            currentPos.z -= 1;
        }
        public void MoveLeft()
        {
            currentPos.x -= 1;
        }
        public void MoveRight()
        {
            currentPos.x += 1;
        }

        public Direction GetSuggestedDirection(Vector3 realPosition)
        {
            // Find the axis differences 
            float diffX = realPosition.x - currentPos.x;
            float diffZ = realPosition.z - currentPos.z;
            float negligibleThreshold = 1f;

            // Choose largest difference
            Debug.Log("choosing largest difference");
            if (Mathf.Abs(diffX) > Mathf.Abs(diffZ))
            {
                Debug.Log("x chosen");
                // The difference on the X axis is the largest of the three differences
                // Check if the difference is negligible
                if (Mathf.Abs(diffX) < negligibleThreshold)
                {
                    Debug.Log("difference negligible");
                    Debug.Log("suggesting none");
                    return Direction.None;
                }

                if (realPosition.x > currentPos.x)
                {
                    Debug.Log("suggesting left");
                    return Direction.Left;
                }
                else
                {
                    Debug.Log("suggesting right");
                    return Direction.Right;
                }
            }
            else if (Mathf.Abs(diffZ) > Mathf.Abs(diffX))
            {
                Debug.Log("z chosen");
                // The difference on the Z axis is the largest of the three differences
                // Check if the difference is negligible
                if (Mathf.Abs(diffZ) < negligibleThreshold)
                {
                    Debug.Log("difference negligible");
                    Debug.Log("suggesting none");
                    return Direction.None;
                }
                if (realPosition.z > currentPos.z)
                {
                    Debug.Log("suggesting backwards");
                    return Direction.Backwards;
                }
                else
                {
                    Debug.Log("suggesting forwards");
                    return Direction.Forwards;
                }
            }
            else
            {
                // All differences are totally equal.... Impressive..
                Debug.Log("all differences are equal... WOWOWOWOOWOWOWOWOWO..");
                Debug.Log("suggesting none");
                return Direction.None;
            }
        }

        private Vector3 Translate(Vector2 vector2)
        {
            return new Vector3(startPos.x + (vector2.x * modifyer), startPos.y, startPos.z + (vector2.y * modifyer));
        }

        private Vector2 Translate(Vector3 vector3)
        {
            return new Vector2(vector3.x - startPos.x, vector3.z - startPos.z);
        }

    }

    public enum Direction
    {
        Forwards,
        Backwards,
        Left,
        Right,
        None
    }
}
