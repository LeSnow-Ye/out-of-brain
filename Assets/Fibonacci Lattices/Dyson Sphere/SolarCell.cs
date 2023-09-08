using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FibonacciLattices;

[ExecuteInEditMode]
public class SolarCell : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent is not null)
        {
            transform.LookAt(transform.parent);
        }
        else
        {
            transform.LookAt(Vector3.zero);
        }
    }
}

