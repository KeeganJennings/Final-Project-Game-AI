using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurAI : FSM
{
    //State Enum
    public enum MinotaurState
    {
        Patrol,
        Smell,
        Sight,
        Attack
    }

    //State
    public MinotaurState curState;

    //Needed Variables for movement
    private float curSpeed;
    private float wallDistance;

    //A* Variables
    Vector3 currentLocation; //Current Actor Location
    Vector3 start; //Starting Location
    Vector3 target; //Where we are heading
    List<Vector3> openList = new List<Vector3>(); //Possible places to move
    List<Vector3> closedList = new List<Vector3>(); //Places the agent has moved to
    public static List<LocationNodes> doneList = new List<LocationNodes>(); //Most effecient move list 


    //Rigid Body
    new private Rigidbody rigidbody;

    //Things needed from the beginning
    protected override void Initialize()
    {
        curState = MinotaurState.Patrol;
        curSpeed = 2.5f;
        wallDistance = 1.5f;
        start = this.transform.position;
        openList.Add(start);
    }

    //Updating State
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


    //Randomly Choosing what direction to go
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


    //Moving Forward for patroling
    private void MoveForward()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    } 

    //Compute H Score
    private static int ComputeHScore(int x, int y, int targetX, int targetY)
    {
        return Math.Abs(targetX - x) + Math.Abs(targetY - y);
    }

    public static void 
}
