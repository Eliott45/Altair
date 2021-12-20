using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Header set in Inspector: ")]
    [SerializeField] private Camera _camera;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _runSpeed = 5f; 
    [SerializeField] private float _walkSpeed = 2.5f;

    [Header("Set Dynamically")] 
    [SerializeField] private EPlayerStates _state = EPlayerStates.Idle;
    
    private Vector2 _smoothDeltaPosition = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Walk = Animator.StringToHash("Walk");

    private void Start() => _agent.updatePosition = false;

    private void Update()
    {
        if (Input.GetAxis("Fire1") >= 1) Move();
        if (_agent.hasPath) AnimateMove();
        UpdateAnimatorState(_state);
    }

    /// <summary>
    /// Update position to agent position.
    /// </summary>
    public void OnAnimatorMove () => transform.position = _agent.nextPosition;

    private void Move()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit)) return;
        _agent.SetDestination(hit.point);
        
        var position = _agent.transform.position;
        var longDistance = Math.Abs((position - hit.point).x) > 3f || 
                       Math.Abs((position - hit.point).z) > 3f || 
                       Math.Abs((position - hit.point).y) > 3f;
        _state = longDistance ? EPlayerStates.Run : EPlayerStates.Walk;
        _agent.speed = longDistance ? _runSpeed : _walkSpeed;
    }

    /// <summary>
    /// Animate character when moving.
    /// </summary>
    private void AnimateMove()
    {
        var worldDeltaPosition = _agent.nextPosition - transform.position;
        
        // Map 'worldDeltaPosition' to local space
        var dx = Vector3.Dot (transform.right, worldDeltaPosition);
        var dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        var smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        _smoothDeltaPosition = Vector2.Lerp (_smoothDeltaPosition, deltaPosition, smooth);
        
        // Update velocity if delta time is safe
        if (Time.deltaTime > 1e-5f) _velocity = _smoothDeltaPosition / Time.deltaTime;
        
        var moving = _velocity.magnitude > 0f && _agent.remainingDistance > (_agent.radius / 3);
        if (!moving) _state = EPlayerStates.Idle;
    }

    private void UpdateAnimatorState(EPlayerStates state)
    {
        switch (state)
        {
            case EPlayerStates.Idle:
                _animator.SetBool(Run, false);
                _animator.SetBool(Walk, false);
                break;
            case EPlayerStates.Walk:
                _animator.SetBool(Run, false);
                _animator.SetBool(Walk, true);
                break;
            case EPlayerStates.Run:
                _animator.SetBool(Run, true);
                _animator.SetBool(Walk, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}
