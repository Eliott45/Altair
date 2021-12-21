using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Set in Inspector:")]
    [Header("Main options:")]
    [SerializeField] private Transform _target;
    [Range(0.01f, 1.0f)]
    [SerializeField] private float _smooth = 0.5f;
    [Header("Rotate options:")]
    [SerializeField] private float _rotateSpeed = 5f;
    [Header("Zoom options:")]
    [SerializeField] private float _zoomSpeed = 0.5f;
    [SerializeField] private float _maxZoomOut = 7.9f;
    [SerializeField] private float _maxZoomIn = 3.5f;

    private Vector3 _cameraOffset;
    
    private void Start()
    {
        _cameraOffset = transform.position - _target.position;
    }
    
    private void LateUpdate()
    {
        if (Input.GetAxis("Fire3") >= 1) Rotate();

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
        
        Follow();
    }

    private void Follow()
    {
        var newPos = _target.position + _cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, _smooth);
        transform.LookAt(_target);
    }

    private void Rotate()
    {
        var camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * _rotateSpeed, Vector3.up);
        _cameraOffset = camTurnAngle * _cameraOffset;
    }
    
    private void Zoom(float zoom)
    {
        if(zoom == 0) return;

        if (zoom > 0 && transform.position.y >= _maxZoomIn)
        {
            _cameraOffset  = new Vector3(_cameraOffset.x, _cameraOffset.y - (zoom + _zoomSpeed), 
                _cameraOffset.z);
        }
        else if(zoom < 0 && transform.position.y <= _maxZoomOut)
        {
            _cameraOffset= new Vector3(_cameraOffset.x, _cameraOffset.y + (-zoom + _zoomSpeed), 
                _cameraOffset.z);
        }
    }
}
