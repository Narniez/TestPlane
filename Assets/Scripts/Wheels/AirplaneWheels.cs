using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WheelCollider))]
public class AirplaneWheels : MonoBehaviour
{
    private WheelCollider wheelCollider;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    public void InitializeWheel()
    {
        if (wheelCollider)
        {
            wheelCollider.motorTorque = 0.000000000001f;
        }
    }
}
