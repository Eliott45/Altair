using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Header set in Inspector: ")]
    [SerializeField] private Camera _camera;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            _navMeshAgent.SetDestination(hit.point);
        }
    }
}
