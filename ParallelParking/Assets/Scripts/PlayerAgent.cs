using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static UnityEngine.GraphicsBuffer;

public class PlayerAgent : Agent
{
    [SerializeField]
    public Transform targetTransform;

    private Vector3 prevTransformPosition;

    public override void Initialize()
    {
        Vector3 newPosition = new Vector3(7.9f, 0.5f, 0f);
        transform.position = newPosition;
    }

    public override void OnEpisodeBegin()
    {

        // Reset the position of the agent to a random position within the room radius
        Vector3 newPosition = new Vector3(7.9f, 0.5f, 0f);
        transform.position = newPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the discrete action index from the actionBuffers
        int discreteAction = actions.DiscreteActions[0];
        float moveSpeed = 5f;

        // Move the agent in the direction of the action
        switch (discreteAction)
        {
            case 0:
                // Move left
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                break;
            case 1:
                // Move right
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                break;
            case 2:
                // Move back
                transform.position += Vector3.back * moveSpeed * Time.deltaTime;
                break;
            case 3:
                // Move right
                transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
                break;
        }

        //float reward = 0.0f;
        //// Розрахунок нагороди на основі зменшення відстані до отвору
        //// Рахуємо нагороду на основі зменшення відстані до отвору
        //float prevDistance = Vector3.Distance(prevTransformPosition, targetTransform.position);
        //float currentDistance = Vector3.Distance(transform.position, targetTransform.position);
        //reward = (prevDistance - currentDistance) / (2 * 12); // Нормалізована нагорода
        //                                                              //Debug.Log("prevDistance - currentDistance: " + (prevDistance - currentDistance));
        //                                                              //AddReward(reward);

        //float prevDistance = Vector3.Distance(prevTransformPosition, targetTransform.position);
        //float currentDistance = Vector3.Distance(transform.position, targetTransform.position);
        //float reward = (prevDistance - currentDistance) / (2 * roomRadius); // Нормалізована нагорода

        //if (currentDistance > roomRadius)
        //{
        //    // End the episode and start a new one
        //    Debug.Log("End episode");
        //    EndEpisode();
        //    return;
        //}


        ////// Calculate the reward for the agent based on its distance from the target
        //Debug.Log(currentDistance);
        ////float reward = -distanceToTarget / 10f; // negative reward for being far from target
        //Debug.Log(reward);
        //SetReward(reward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Goal>(out Goal goal)) {
            SetReward(1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 4;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 3;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActions[0] = 2;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActions[0] = 0;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActions[0] = 1;
        }
    }
}
