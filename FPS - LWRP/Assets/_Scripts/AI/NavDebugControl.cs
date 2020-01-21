using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavDebugControl : MonoBehaviour
{
    private NavMeshAgent NavAgent;
    public bool enable;

    public Vector3 ClickPos;

    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && enable)
        {
            ClickPos = ClickPosition();
            MoveAgent(NavAgent);
        }
    }

    private void MoveAgent(NavMeshAgent agent)
    {
        if (!NavAgent) return;

        agent.SetDestination(ClickPos);
    }

    private Vector3 ClickPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return !Physics.Raycast(ray, out var hit, Mathf.Infinity) ? new Vector3(0, 0, 0) : hit.point;

        //Debug.Log(hit.point);
    }

}
