using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CarAgent : Agent
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private BehaviorParameters behaviorParameters;
    private CarController carController;

    [SerializeField]
    private Collider parkingCollider;

    [SerializeField]
    private Collider deeperParkingCollider;

    public override void Initialize()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.rotation;
        behaviorParameters = GetComponent<BehaviorParameters>();
        carController = GetComponent<CarController>();
        carController.IsAutonomous = behaviorParameters.BehaviorType == BehaviorType.Default;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = originalPosition;
        transform.rotation = originalRotation;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(deeperParkingCollider.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the discrete action index from the actionBuffers
        int discreteAction = actions.DiscreteActions[0];
        int discrete1 = actions.DiscreteActions[1];
        float discrete2 = actions.DiscreteActions[2];
        float horizontal = 0f;
        switch (discrete1)
        {
            case 0:
                horizontal = -1f;
                break;
            case 1:
                horizontal = 0f;
                break;
            case 2:
                horizontal = 1f;
                break;
        }
        float vertical = 0f;
        switch (discrete2)
        {
            case 0:
                vertical = -1f;
                break;
            case 1:
                vertical = 0f;
                break;
            case 2:
                vertical = 1f;
                break;
        }

        bool isBraking = discreteAction == 1;

        carController.SetInputs(horizontal, vertical, isBraking);

        int counter = 0;

        Bounds parkingBounds = parkingCollider.bounds;
        Bounds deeperParkingBounds = deeperParkingCollider.bounds;
        Bounds carBounds = GetComponent<Collider>().bounds;

        Vector3[] carCorners = new Vector3[4];
        carCorners[0] = carBounds.center + new Vector3(-carBounds.extents.x, 0, -carBounds.extents.z);
        carCorners[1] = carBounds.center + new Vector3(-carBounds.extents.x, 0, carBounds.extents.z);
        carCorners[2] = carBounds.center + new Vector3(carBounds.extents.x, 0, -carBounds.extents.z);
        carCorners[3] = carBounds.center + new Vector3(carBounds.extents.x, 0, carBounds.extents.z);

        float additionalReward = 0f;
        foreach (Vector3 corner in carCorners)
        {
            if (parkingBounds.Contains(corner))
            {
                counter++;
                additionalReward += 0.5f / MaxStep;
            }
            /*if(deeperParkingBounds.Contains(corner))
            {
                additionalReward += 1f / MaxStep;
            }*/
        }

        if (counter == carCorners.Length)
        {
            float currReward = GetCumulativeReward();
            SetReward(Mathf.Max(10f, currReward + 10f));
            Debug.Log("Success");
            EndEpisode();
        }
        else if(counter > 0)
        {
            additionalReward += 1f / MaxStep / Vector3.Distance(deeperParkingCollider.transform.localPosition, transform.localPosition);
            if(discrete2 == 1 || isBraking)
            {
                additionalReward /= 2;
            }
        }
        AddReward(additionalReward);
        AddReward(-1f / MaxStep);
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.transform.tag.ToLower();
        if (tag == "barrier")
        {
            Debug.Log("Barrier hit");
            AddReward(-0.2f);
            EndEpisode();
        }
        else if (tag == "car")
        {
            Debug.Log("Car hit");
            AddReward(-0.2f);
            EndEpisode();
        }
        else if (tag == "goal")
        {
            Debug.Log("Goal hit");
            /*AddReward(0.01f);
            EndEpisode();*/
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.transform.tag.ToLower();
        if (tag == "car")
        {
            Debug.Log("Car hit");
            AddReward(-0.2f);
            EndEpisode();
        }
    }
}