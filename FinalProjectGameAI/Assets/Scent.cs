using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : MonoBehaviour
{
    public float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = 15.5f;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        DestroyOnOutOfTime();
    }

    private void DestroyOnOutOfTime()
    {
        if(timeLeft <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
