using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed;
    private float timeSinceLastDeployedCollider;

    public GameObject scentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey("w"))
        {
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey("s"))
        {
            transform.position -= transform.TransformDirection(Vector3.forward) * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey("a"))
        {
            transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey("d"))
        {
            transform.position -= transform.TransformDirection(Vector3.left) * Time.deltaTime * moveSpeed;
        }

        UpdateTime();
        if(timeSinceLastDeployedCollider >= 3)
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
