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
    private float timeSinceLastMove = 0;
    private int index = 0;
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
   

    //A* Variables
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public Transform startPos, endPos;
    [SerializeField]
    public ArrayList pathArray;
    GameObject player;


    //Rigid Body
    new private Rigidbody rigidbody;

    //Things needed from the beginning
    protected override void Initialize()
    {
        curState = MinotaurState.Patrol;
        curSpeed = 2.5f;
        wallDistance = 1.5f;
        pathArray = new ArrayList();
        player = GameObject.FindGameObjectWithTag("End");
    }

    //Updating State
    protected override void FSMFixedUpdate()
    {
        switch (curState)
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

        SetPath();

        elaspedTime += Time.deltaTime;
    }

    private void UpdateSightState()
    {
        FollowPath();
    }

    private void UpdateSmellState()
    {
        FollowSmell();
    }

    private void UpdateAttackState()
    {
        Attack();
    }

    private void UpdatePatrolState()
    {
        RaycastHit hit;
        Ray wallIntercept = new Ray(transform.position, this.transform.forward * wallDistance);

        Debug.DrawRay(transform.position, this.transform.forward * wallDistance);


        if (Physics.Raycast(wallIntercept, out hit, wallDistance))
        {
            if (hit.collider.tag == "Wall")
            {
                MakeTurnDecision();
            }
            if(hit.collider.tag == "Scent")
            {
                curState = MinotaurState.Smell;
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

    private void SetPath()
    {
        startPos = this.transform;
        endPos = player.transform;

        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));

        pathArray = AStar.FindPath(startNode, goalNode);
    }

    private void FollowPath()
    {
        if (index != pathArray.Count && timeSinceLastMove >= .075)
        {
            Node curNode = FindNextNode();

            Vector3 moveTowardsVector = new Vector3(curNode.position.x, transform.position.y, curNode.position.z);

            transform.position = Vector3.MoveTowards(transform.position, moveTowardsVector, curSpeed - 2);

            Debug.Log("I Am Moving + " + curNode.position);

            timeSinceLastMove = 0;

            SetPath();
        }
        else if(index == pathArray.Count)
        {
            curState = MinotaurState.Attack;
            index = 0;
        }

        timeSinceLastMove += Time.deltaTime;
        Debug.Log(timeSinceLastMove);
    }

    private Node FindNextNode()
    {
        Node node = new Node();

        if(index <= pathArray.Count)
        {
            node = (Node)pathArray[index];
        }

        if(this.transform.position.x == node.position.x && this.transform.position.z == node.position.z)
        {
            index++;
            Debug.Log("Updated index");
        }

        return node;
    }

    private void OnDrawGizmos()
    {
        if(pathArray == null)
        {
            return;
        }

        if(pathArray.Count > 0)
        {
            int index = 1;

            foreach(Node node in pathArray)
            {
                if(index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.red);
                    index++;
                }
            }
        }
    }

    private void Attack()
    {
        curState = MinotaurState.Patrol;
    }

    private void FollowSmell()
    {
        GameObject[] smellNodes = GameObject.FindGameObjectsWithTag("Scent");
        GameObject moveToNode = GetClosestSmell(smellNodes);

        transform.position = Vector3.SmoothDamp(transform.position, moveToNode.transform.position, ref velocity, smoothTime);

    }

    private GameObject GetClosestSmell(GameObject[] nodes)
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = this.transform.position;

        foreach(GameObject node in nodes)
        {
            float dist = Vector3.Distance(node.transform.position, currentPos);

            if(dist < minDist)
            {
                tMin = node;
                minDist = dist;
            }
        }

        return tMin;
    }
}
