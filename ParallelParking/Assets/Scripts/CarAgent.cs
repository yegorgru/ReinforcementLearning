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
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the discrete action index from the actionBuffers
        int discreteAction = actions.DiscreteActions[0];
        float horizontal = actions.ContinuousActions[0];
        float vertical = actions.ContinuousActions[1];

        bool isBraking = discreteAction == 1;

        carController.SetInputs(horizontal, vertical, isBraking);

        int counter = 0;

        Bounds parkingBounds = parkingCollider.bounds;
        Bounds carBounds = GetComponent<Collider>().bounds;

        Vector3[] carCorners = new Vector3[4];
        carCorners[0] = carBounds.center + new Vector3(-carBounds.extents.x, 0, -carBounds.extents.z);
        carCorners[1] = carBounds.center + new Vector3(-carBounds.extents.x, 0, carBounds.extents.z);
        carCorners[2] = carBounds.center + new Vector3(carBounds.extents.x, 0, -carBounds.extents.z);
        carCorners[3] = carBounds.center + new Vector3(carBounds.extents.x, 0, carBounds.extents.z);

        foreach (Vector3 corner in carCorners)
        {
            if (parkingBounds.Contains(corner))
            {
                counter++;
            }
        }

        if (counter == carCorners.Length)
        {
            float currReward = GetCumulativeReward();
            SetReward(Mathf.Max(1f, currReward + 1));
            Debug.Log("Success");
            EndEpisode();
        }
        else
        {
            AddReward(counter * 1f / MaxStep);
        }


        AddReward(-1f / MaxStep);
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.transform.tag.ToLower();
        if(tag == "barrier")
        {
            Debug.Log("Barrier hit");
            AddReward(-0.5f);
            EndEpisode();
        }
        else if(tag == "car")
        {
            Debug.Log("Car hit");
            AddReward(-1f);
            EndEpisode();
        }
        else if(tag == "goal")
        {
            Debug.Log("Goal hit");
            /*AddReward(0.01f);
            EndEpisode();*/
        }
    }
}