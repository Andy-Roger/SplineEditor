using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //set 0 index to position of cube
        //set 1 index to position of Zero

        GetComponent<LineRenderer>().SetPosition(1, transform.InverseTransformPoint(Vector3.zero));

    }
}
