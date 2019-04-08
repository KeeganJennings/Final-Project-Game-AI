using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable
{
    public float nodeTotalCost;
    public float estimatedCost;
    public bool bObstacle;
    public Node parent;
    public Vector3 position;
    
    public Node(Vector3 pos)
    {
        this.estimatedCost = 0.0f;
        this.nodeTotalCost = 1.0f;
        this.bObstacle = false;
        this.parent = null;
        position = pos;
    }

    public void MarkAsObstacle()
    {
        this.bObstacle = true;
    }

    public int CompareTo(object obj)
    {
        int returnValue = 0;
        Node node = (Node)obj;

        if(this.estimatedCost < node.estimatedCost)
        {
            returnValue = -1;
        }

        if(this.estimatedCost > node.estimatedCost)
        {
            returnValue = 0;
        }

        return returnValue;
    }
}
