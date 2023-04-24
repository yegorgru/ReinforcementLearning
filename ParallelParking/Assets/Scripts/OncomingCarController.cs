using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncomingCarController : CarController
{
    [SerializeField]
    private float spawningZCoord;
    [SerializeField]
    private float finishZCoord;

    public void Start()
    {
        IsAutonomous = false;
        horizontalInput = 0f;
        verticalInput = 0.7f;
        isBraking = false;
        Vector3 newPos = transform.localPosition;
        newPos.z = spawningZCoord;
        transform.localPosition = newPos;
    }

    protected override void CheckConstraints()
    {
        if (transform.localPosition.z <= finishZCoord)
        {
            Vector3 newPos = transform.localPosition;
            newPos.z = spawningZCoord;
            transform.localPosition = newPos;
        }
    }

    protected override void GetInput()
    {
        float minValue = -1f;
        float maxValue = 1f;
        System.Random random = new System.Random();
        float ver = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        verticalInput += ver / 10f;
        verticalInput = Mathf.Min(1f, Mathf.Max(0.4f, verticalInput));
        float brakingHelper = (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        isBraking = brakingHelper > 0.75;
    }
}
