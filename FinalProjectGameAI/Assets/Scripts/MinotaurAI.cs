using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    private bool isInSite;

    //A* Variables
    public static PrioritQueue openList;
    public static HashSet<Node> closedList;
    private Node startNode { get; set; }
    private Node goalNode { get; set; }
    public ArrayList pathArray;


    //Rigid Body
    new private Rigidbody rigidbody;

    //Things needed from the beginning
    protected override void Initialize()
    {
        curState = MinotaurState.Patrol;
        curSpeed = 2.5f;
        wallDistance = 1.5f;
        pathArray = new ArrayList();
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
        var player = GameObject.Find("Player");
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(this.transform.position)));

        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(player.transform.position)));

        FindPath(startNode, goalNode);
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

    private static float HEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    public static ArrayList FindPath(Node start, Node goal)
    {
        openList = new PrioritQueue();
        openList.Push(start);

        start.nodeTotalCost = 0.0f;
        start.estimatedCost = HEstimateCost(start, goal);

        closedList = new HashSet<Node>();
        Node node = null;

        while(openList.Length != 0)
        {
            node = openList.First();
            if(node.position == goal.position)
            {
                return CalculatePath(node);
            }

            ArrayList neighbors = new ArrayList();

            GridManager.instance.GetNeighbors(node, neighbors);

            for(int i = 0; i < neighbors.Count; i++)
            {
                Node neighborNode = (Node)neighbors[i];
                if(!closedList.Contains(neighborNode))
                {
                    float cost = HEstimateCost(node, neighborNode);

                    float totalCost = node.nodeTotalCost + cost;
                    float neighborNodeEstCost = HEstimateCost(neighborNode, goal);

                    neighborNode.nodeTotalCost = totalCost;
                    neighborNode.parent = node;
                    neighborNode.estimatedCost = totalCost + neighborNodeEstCost;

                    if(!openList.Cointains(neighborNode))
                    {
                        openList.Push(neighborNode);
                    }
                }
            }
            closedList.Add(node);
            openList.Remove(node);
        }

        if(node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        return CalculatePath(node);
    }

    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while(node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }
}
