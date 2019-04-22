using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed;
    private float timeSinceLastDeployedCollider;
    Rigidbody rB;
    private Vector3 direction;
    private Vector3 rotationTemp;
    private Transform targetRotation;

    private float moveHorizontal;
    private float moveVertical;
    private float rotationSpeed;

    public GameObject scentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
        //moveSpeed = 7.5f;
        rotationSpeed = 2.5f;
        direction = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        direction.x = direction.z = 0;

        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        RotateThePlayer();

        MoveThePlayer();

        UpdateTime();
        CheckTimeOnCollider();
    }

    private void MoveThePlayer()
    {
        direction = new Vector3(0, 0, moveVertical);
        direction.Normalize();

        if(direction.z > 0)
        {
            rB.AddForce(transform.forward * moveSpeed);
        }
        else if (direction.z < 0)
        {
            rB.AddForce(-transform.forward * moveSpeed);
        }

    }

    private void RotateThePlayer()
    {
        rotationTemp = new Vector3(moveHorizontal, 0, 0);
        rotationTemp.Normalize();
        moveHorizontal = rotationTemp.x;

        transform.Rotate(Vector3.up, (moveHorizontal * rotationSpeed)); 
    }

    private void CheckTimeOnCollider()
    {
        if (timeSinceLastDeployedCollider >= 1)
        {
            SpawnScentCollider();
            timeSinceLastDeployedCollider = 0;
        }
    }

    private void UpdateTime()
    {
        timeSinceLastDeployedCollider += Time.deltaTime;
    }

    private void SpawnScentCollider()
    {
        GameObject scent = Instantiate(scentPrefab);

        scent.transform.position = this.transform.position;
    }
}
