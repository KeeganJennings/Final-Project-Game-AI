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
        curSpeed = 2.5f;
        wallDistance = 1.5f;
    }

    protected override void FSMFixedUpdate()
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
        Ray wallIntercept = new Ray(transform.position,  this.transform.forward * wallDistance);

        Debug.DrawRay(transform.position, this.transform.forward * wallDistance);

        
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
        float yRotation = 90;

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            transform.eulerAngles += new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
        }
       else
       {
                transform.eulerAngles += new Vector3(transform.eulerAngles.x, -yRotation, transform.eulerAngles.z);
       }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
