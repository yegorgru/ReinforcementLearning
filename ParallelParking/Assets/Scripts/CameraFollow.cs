using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float translationSpeed;

    [SerializeField]
    private float rotationSpeed;

    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
    }

    private void HandleTranslation()
    {
        Vector3 targetPosition = target.TransformPoint(offset);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, translationSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        var direction = target.localPosition - transform.localPosition;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, rotationSpeed);
    }
}
