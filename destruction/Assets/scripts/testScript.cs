using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public GameObject target;
    public Transform plane;
    Vector3 entryPoint;
    Vector3 exitPoint;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var slicer = target.GetComponent<Slice>();
            var sliceNormal = Vector3.right;
            var sliceOrigin = target.transform.position;

            slicer.ComputeSlice(sliceNormal, sliceOrigin);
        }
    }


    void OnTriggerEnter(Collider collision)
    {
        if(collision.GetComponent<Slice>() != null)
        {
            entryPoint = transform.position;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<Slice>() != null)
        {
            exitPoint = transform.position;


            //slicing script
            var slicer = collision.gameObject.GetComponent<Slice>();
            plane.forward = (exitPoint - entryPoint);
            var sliceNormal = plane.up;
            var sliceOrigin = Vector3.Lerp(entryPoint, exitPoint, .5f);

            slicer.ComputeSlice(sliceNormal, sliceOrigin);
        }
    }
}
