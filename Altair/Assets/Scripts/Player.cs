using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Header set in Inspector: ")]
    [SerializeField] private Camera _camera;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Animator _animator;

    private static readonly int Move = Animator.StringToHash("Move");
    
    private Vector2 _smoothDeltaPosition = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;

    private void Start()
    {
        _navMeshAgent.updatePosition = false;
    }

    private void Update()
    {
        var worldDeltaPosition = _navMeshAgent.nextPosition - transform.position;
        
        // Map 'worldDeltaPosition' to local space
        var dx = Vector3.Dot (transform.right, worldDeltaPosition);
        var dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        var smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        _smoothDeltaPosition = Vector2.Lerp (_smoothDeltaPosition, deltaPosition, smooth);
        
        // Update velocity if delta time is safe
        if (Time.deltaTime > 1e-5f)
            _velocity = _smoothDeltaPosition / Time.deltaTime;

        var moving = _velocity.magnitude > 0f && _navMeshAgent.remainingDistance > (_navMeshAgent.radius / 3);

        _animator.SetBool(Move, moving);

        if (!Input.GetMouseButtonDown(0)) return;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.SetDestination(hit.point);
    }

    public void OnAnimatorMove () {
        transform.position = _navMeshAgent.nextPosition;  // Update postion to agent position
    }
}
