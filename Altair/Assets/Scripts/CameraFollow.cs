using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Set in Inspector:")] 
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        transform.position = _target.position + offset;
    }

    private void LateUpdate()
    {
        var desirePosition = _target.position + offset;
        var smoothedPosition = Vector3.Lerp(transform.position, desirePosition, _smoothSpeed);

        transform.position = smoothedPosition;
    }
}
