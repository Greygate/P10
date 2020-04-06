using System;
using System.Collections;
using System.Collections.Generic;
using TelloLib;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    [Range(1, 100)]
    public float droneSpeed = 50f;

    [Range(0, 10)]
    public int height = 1;

    public Vector3 startPosition;
    public Vector3 currentPosition;

    private void Start()
    {
        Tello.StartConnecting();
    }

    void Update()
    {
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

        if (Tello.ConnectionState == ConnectionState.Connected && Tello.state.flying)
        {
            Tello.controllerState.SetAxis(Input.GetAxis("Rotate") * droneSpeed / 100, Input.GetAxis("Levitate") * droneSpeed / 100, Input.GetAxis("LRTranslate") * droneSpeed / 100, Input.GetAxis("FBTranslate") * droneSpeed / 100);
            Tello.SetMaxHeight(height);
        }
    }

    //TODO: Implement me
    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }

    //TODO: Implement me
    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

}
