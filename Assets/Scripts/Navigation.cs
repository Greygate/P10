using System;
using System.Collections;
using System.Collections.Generic;
using TelloLib;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    [Range(1, 100)]
    public float movementSpeed = 50f;

    [Range(0, 10)]
    public int height = 1;

    public float timeToTravel = 0.1f;

    float droneMovementSpeed;
    bool busy = false;
    bool positionManagerFirstFire = false;
    bool positionManagerShouldBeRunning = true;
    bool hasSchedulerBeenRun = false; // deleteme
    Queue<Func<IEnumerator>> movements = new Queue<Func<IEnumerator>>(); // deleteme
    NavigationalArray navArray;

    private void Start()
    {
        navArray = new NavigationalArray(GetDronePosition());
        StartCoroutine(DronePositionManager());

        /*
        Tello.onUpdate += (int newState) =>
        {
            if (!positionManagerShouldBeRunning && Tello.state.flying)
            {
                positionManagerShouldBeRunning = true;
                
            }
            else if (positionManagerShouldBeRunning && !Tello.state.flying)
                positionManagerShouldBeRunning = false;
                
        };
        */

        Tello.StartConnecting();
    }

    void Update()
    {
        droneMovementSpeed = movementSpeed / 100;
        Tello.SetMaxHeight(height);
    }

    public void MoveForwards()
    {
        Debug.Log("Navigation: Moving forwards");
        navArray.MoveForwards();
    }
    public void MoveBackwards()
    {
        navArray.MoveBackwards();
    }
    public void MoveRight()
    {
        navArray.MoveRight();
    }
    public void MoveLeft()
    {
        navArray.MoveLeft();
    }

    public Vector3 GetCurrentPosition()
    {
        return GetDronePosition() - navArray.startPos;
    }
    public Vector3 GetWantedPosition()
    {
        return navArray.currentPos;
    }
    public Vector3 GetStartPosition()
    {
        return navArray.startPos;
    }

    IEnumerator PerformMoveForwards()
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

    IEnumerator PerformMoveBackwards()
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

    IEnumerator PerformMoveLeft()
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

    IEnumerator PerformMoveRight()
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

    IEnumerator Scheduler()
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

    IEnumerator DronePositionManager()
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
                    StartCoroutine(PerformMoveForwards());
                    break;
                case Direction.Backwards:
                    StartCoroutine(PerformMoveBackwards());
                    break;
                case Direction.Left:
                    StartCoroutine(PerformMoveLeft());
                    break;
                case Direction.Right:
                    StartCoroutine(PerformMoveRight());
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

    void WallFollowerPathFinding()
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

        public void MoveForwards()
        {
            Debug.Log("NavArray: Moving forwards");
            Debug.Log($"NavArray: First position: {currentPos}");
            currentPos.z += 1;
            Debug.Log($"NavArray: Second position: {currentPos}");
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
