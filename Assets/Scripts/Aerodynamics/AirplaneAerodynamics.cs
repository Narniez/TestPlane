using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AirplaneAerodynamics : MonoBehaviour
{
    // Start is called before the first frame update
    public float forwardSpeed;
    public float kph;
    public float maxKph = 178f;
    private float maxMPS;
    private float normalizedKph;

    //LIFT
    public float maxLiftPower = 800f;
    public AnimationCurve liftCurve = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public float pitchSpeed = 1000f;
    public float rollSpeed = 1000f;
    public float yawSpeed = 1000f;

    private BaseAirplaneInputs airplaneInputs;

    //DRAG
    public float dragFactor = 0.01f;

    //ANGLE OF ATTACK
    private float angleOfAttack;
    private float pitchAngle;
    private float rollAngle;


    private Rigidbody rb;
    private float startDrag;
    private float startAngularDrag;


    const float mpsToKph = 3.6f;
    public void InitializeAerodynamics(Rigidbody rigidBody, BaseAirplaneInputs inputs)
    {
        airplaneInputs = inputs;
        rb = rigidBody;
        startDrag = rb.drag;
        startAngularDrag = rb.angularDrag;

        maxMPS = maxKph / mpsToKph;
    }

    public void UpdateAerodynamics()
    {
        if (rb)
        {
            CalculateForwardSpeed();
            CalculateLift();
            CalculateDrag();
            HandlePitch();
            HandleRoll();
            HandleYaw();

            HandleRigibodyTransform();
        }
    }

    void CalculateForwardSpeed()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        forwardSpeed = Mathf.Max(0, localVelocity.z);
        forwardSpeed = Mathf.Clamp(forwardSpeed, 0, maxKph);

        kph = forwardSpeed * mpsToKph;
        kph = Mathf.Clamp(kph, 0, maxKph);
        normalizedKph = Mathf.InverseLerp(0, maxKph, kph);

        //Debug.Log(normalizedKph);

        //Debug.DrawRay(transform.position, transform.position + localVelocity, Color.green);
    }

    void CalculateLift()
    {
        //get the angle of attack
        angleOfAttack = Vector3.Dot(rb.velocity.normalized, transform.forward);

        //Make it more of a curve instead of a linear value for a more realistic feel
        angleOfAttack *= angleOfAttack;


        //Debug.Log("Angle of attack: " + angleOfAttack);
        Vector3 liftDirection = transform.up;

        //Scalar value that we can use for the lift direction
        float liftPower = liftCurve.Evaluate(normalizedKph) * maxLiftPower;
        //Debug.Log(liftPower);

        Vector3 finalLiftForce = liftDirection * liftPower * angleOfAttack;

        rb.AddForce(finalLiftForce);
    }

    void CalculateDrag()
    {
        float speedDrag = forwardSpeed * dragFactor;
        float finalDrag = startDrag + speedDrag;

        rb.drag = finalDrag;
        rb.angularDrag = startAngularDrag * forwardSpeed;
    }

    //Rotate the rigidbody towards the direction the engine is pulling it  
    void HandleRigibodyTransform()
    {
        //Check if we are in motion 
        if (rb.velocity.magnitude > 1f)
        {
            // Calculate the corrective force instead of overwriting velocity
            Vector3 forwardVelocity = Vector3.Project(rb.velocity, transform.forward);
            Vector3 correctiveForce = (forwardVelocity - rb.velocity) * 0.5f; // Adjust this factor to control the strength

            // Apply the corrective force gradually
            rb.AddForce(correctiveForce, ForceMode.Acceleration);
        }
    }

    void HandlePitch()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;
        flatForward = flatForward.normalized;

        pitchAngle = Vector3.Angle(transform.forward, flatForward);

        //Debug.Log("Pitch Angle: " + pitchAngle);

        //
        Vector3 pitchTorque = airplaneInputs.Pitch * pitchSpeed * transform.right;

        rb.AddTorque(pitchTorque);
    }

    void HandleRoll()
    {
        Vector3 flatRight = transform.right;
        flatRight.y = 0f;
        flatRight = flatRight.normalized;

        rollAngle = Vector3.Angle(transform.right, flatRight);

        Vector3 rollTorque = -airplaneInputs.Roll * rollSpeed * transform.forward;

        rb.AddTorque(rollTorque);
    }

    void HandleYaw()
    {
        Vector3 yawTorque = airplaneInputs.Yaw * yawSpeed * transform.up;

        rb.AddTorque(yawTorque);
    }
}