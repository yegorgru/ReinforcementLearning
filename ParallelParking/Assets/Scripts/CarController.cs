using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private float steerAngle;

    [SerializeField]
    private float motorForce;
    [SerializeField]
    private float brakeForce;
    [SerializeField]
    private float maxSteerAngle;

    [SerializeField]
    private WheelCollider flWheelCollider;
    [SerializeField]
    private WheelCollider rlWheelCollider;
    [SerializeField]
    private WheelCollider frWheelCollider;
    [SerializeField]
    private WheelCollider rrWheelCollider;

    [SerializeField]
    private Transform flWheelTransform;
    [SerializeField]
    private Transform rlWheelTransform;
    [SerializeField]
    private Transform frWheelTransform;
    [SerializeField]
    private Transform rrWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        flWheelCollider.motorTorque = verticalInput * motorForce;
        frWheelCollider.motorTorque = verticalInput * motorForce;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        float currBrakeForce = isBraking ? brakeForce : 0f;
        flWheelCollider.brakeTorque = currBrakeForce;
        frWheelCollider.brakeTorque = currBrakeForce;
        rlWheelCollider.brakeTorque = currBrakeForce;
        rrWheelCollider.brakeTorque = currBrakeForce;
    }

    private void HandleSteering()
    {
        steerAngle = maxSteerAngle * horizontalInput;
        flWheelCollider.steerAngle = steerAngle;
        frWheelCollider.steerAngle = steerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(flWheelCollider, flWheelTransform);
        UpdateSingleWheel(rlWheelCollider, rlWheelTransform);
        UpdateSingleWheel(frWheelCollider, frWheelTransform);
        UpdateSingleWheel(rrWheelCollider, rrWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion quater;
        collider.GetWorldPose(out pos, out quater);
        transform.transform.rotation = quater;
        transform.transform.position = pos;
    }
}