using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake : MonoBehaviour
{
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
    private Node startNode { get; set; }
    private Node goalNode { get; set; }
    private Transform startPos, endPos;
    [SerializeField]
    public ArrayList pathArray;
    GameObject player;


    //Rigid Body
    new private Rigidbody rigidbody;

    //Things needed from the beginning
    protected void Initialize()
    {
        curState = MinotaurState.Patrol;
        curSpeed = 2.5f;
        wallDistance = 1.5f;
        pathArray = new ArrayList();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    //Updating State
    protected void Update()
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

        startPos = this.transform;
        endPos = player.transform;

        SetPath();

        //elaspedTime += Time.deltaTime;
    }

    private void UpdateSightState()
    {
        FollowPath();
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
        Ray wallIntercept = new Ray(transform.position, this.transform.forward * wallDistance);

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

    private void SetPath()
    {

        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));

        pathArray = AStar.FindPath(startNode, goalNode);

        for (int i = 0; i < pathArray.Count; i++)
        {
            Node node = (Node)pathArray[i];
            Debug.Log(goalNode.position);
            Debug.Log(node.position);
        }

    }

    private void FollowPath()
    {

    }

    private void OnDrawGizmos()
    {
        if (pathArray == null)
        {
            return;
        }

        if (pathArray.Count > 0)
        {
            int index = 1;

            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.red);
                    index++;
                }
            }
        }
    }
}
