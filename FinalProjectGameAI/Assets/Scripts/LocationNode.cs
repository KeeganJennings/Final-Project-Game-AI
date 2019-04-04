using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationNodes : MonoBehaviour
{
    public Vector3 location;
    public double G, H, F;
    public LocationNodes parent;

    public void CallParent()
    {
        MinotaurAI.doneList.Add(this);
    }
}
