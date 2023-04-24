using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public float horizontalInput { get; set; }
    public float verticalInput { get; set; }
    public bool isBraking { get; set; }
    private float steerAngle;

    public bool IsAutonomous { get; set; } = false;

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
        if (!IsAutonomous)
        {
            GetInput();
        }
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    public void SetInputs(float horizontalInput, float verticalInput, bool isBraking)
    {
        this.horizontalInput = horizontalInput;
        this.verticalInput = verticalInput;
        this.isBraking = isBraking;
    }

    private void GetInput()
    {
        horizontalInput = Mathf.Round(Input.GetAxis(HORIZONTAL));
        verticalInput = Mathf.Round(Input.GetAxis(VERTICAL));
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