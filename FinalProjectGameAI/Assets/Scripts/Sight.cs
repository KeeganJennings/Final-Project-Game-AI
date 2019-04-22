using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "End")
        {
            MinotaurAI parentAI = GetComponentInParent<MinotaurAI>();

            parentAI.curState = MinotaurAI.MinotaurState.Sight;
        }
        else
        {
            MinotaurAI parentAI = GetComponentInParent<MinotaurAI>();

            parentAI.curState = MinotaurAI.MinotaurState.Patrol;
        }
    }
}
