using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurAI : FSM
{
    public enum MinotaurState
    {
        Patrol,
        Smell,
        Sight,
        Attack
    }

    public MinotaurState curState;

    private float curSpeed;
    private float wallDistance;

    new private Rigidbody rigidbody;

    protected override void Initialize()
    {
        curState = MinotaurState.Patrol;
        curSpeed = 10f;
        wallDistance = 40f;
    }

    protected override void FSMUpdate()
    {
        switch(curState)
        {
            case MinotaurState.Patrol:
                UpdatePatrolState();
                break;
            case MinotaurState.Attack:
                UpdateAttackState();
                break;
            case MinotaurState.Smell:
                UpdateSmellState();
                break;
            case MinotaurState.Sight:
                UpdateSightState();
                break;
        }

        elaspedTime += Time.deltaTime;
    }

    private void UpdateSightState()
    {
        
    }

    private void UpdateSmellState()
    {
        
    }

    private void UpdateAttackState()
    {
        
    }

    private void UpdatePatrolState()
    {
        RaycastHit hit;
        Ray wallIntercept = new Ray(transform.position, Vector3.forward);

        Debug.DrawRay(transform.position, Vector3.forward * wallDistance);

        if (Physics.Raycast(wallIntercept, out hit, wallDistance))
        { 
            if (hit.collider.tag == "Wall")
            {
                MakeTurnDecision();
            }
        }
        else
        {
            MoveForward();
        }
    }

    private void MakeTurnDecision()
    {
        if(UnityEngine.Random.Range(0, 1) == 0)
        {
            transform.Rotate(0, 90, 0, Space.Self);
        }
        else
        {
            transform.Rotate(0, -90, 0, Space.Self);
        }

    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
