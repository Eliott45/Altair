using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Header set in Inspector: ")]
    [SerializeField] private Camera _camera;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    private Vector2 _smoothDeltaPosition = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Distance = Animator.StringToHash("Distance");

    private void Start() => _agent.updatePosition = false;

    private void Update()
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
        if (Time.deltaTime > 1e-5f)
            _velocity = _smoothDeltaPosition / Time.deltaTime;

        var moving = _velocity.magnitude > 0f && _agent.remainingDistance > (_agent.radius / 3);

        _animator.SetBool(Move, moving);

        if (!Input.GetMouseButtonDown(0)) return;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit)) return;
        _agent.SetDestination(hit.point);
        
        var position = _agent.transform.position;
        _animator.SetFloat(Distance, Math.Abs((position - hit.point).x) > 3f || Math.Abs((position - hit.point).z) > 2.5f
            ? 1 : 0);
    }

    /// <summary>
    /// Update position to agent position.
    /// </summary>
    public void OnAnimatorMove () => transform.position = _agent.nextPosition;  
    
}
