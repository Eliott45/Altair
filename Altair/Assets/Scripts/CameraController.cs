using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [SerializeField] private Transform _target;
    [Range(0.01f, 1.0f)]
    [SerializeField] private float _smooth = 0.5f;
    [SerializeField] public float _rotateSpeed = 5f;

    private Vector3 _cameraOffset;
    
    private void Start()
    {
        _cameraOffset = transform.position - _target.position;
    }
    
    private void LateUpdate()
    {
        if (Input.GetAxis("Fire3") >= 1) RotateCamera();

        var newPos = _target.position + _cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, _smooth);
        transform.LookAt(_target);
    }

    private void RotateCamera()
    {
        var camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * _rotateSpeed, Vector3.up);
        _cameraOffset = camTurnAngle * _cameraOffset;
    }
}
