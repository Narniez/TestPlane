using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AirplaneAerodynamics))]
public class AirplaneController : RigidBodyController
{

    public BaseAirplaneInputs Input;
    public AirplaneAerodynamics airplaneAerodynamics;
    public Transform centerOfGravity;
    public float airplaneWeight = 800f;


    public List<AirplaneEngine> engines = new List<AirplaneEngine>();

    public List<AirplaneWheels> wheels = new List<AirplaneWheels>();

    public override void Start()
    {
        base.Start();

        if (rb)
        {
            rb.mass = airplaneWeight;

            if (centerOfGravity)
            {
                rb.centerOfMass = centerOfGravity.localPosition;
            }

            airplaneAerodynamics = GetComponent<AirplaneAerodynamics>();
            if (airplaneAerodynamics)
            {
                airplaneAerodynamics.InitializeAerodynamics(rb, Input);
            }
        }

        if (wheels != null)
        {
            if (wheels.Count > 0)
            {
                foreach (AirplaneWheels wheel in wheels)
                {
                    wheel.InitializeWheel();
                }
            }
        }

       
    }
    protected override void HandlePhysics()
    {
        if (Input)
        {
            HandleEngines();
            HandleAerodynamics();
            HandleSteering();
            HandleBreaks();
            HandleAltitude();
        }
    }

    void HandleEngines()
    {
        if (engines != null)
        {
            if (engines.Count > 0)
            {
                foreach (AirplaneEngine engine in engines)
                {
                    rb.AddForce(engine.CalculateForce(Input.StickyThrottle));

                }
            }
        }
    }

    void HandleAerodynamics()
    {
        airplaneAerodynamics.UpdateAerodynamics();
    }

    void HandleSteering()
    {

    }

    void HandleBreaks()
    {

    }

    void HandleAltitude()
    {

    }
}
