using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class testScript : MonoBehaviour
{
    Transform plane;
    Vector3 entryPoint;
    Vector3 exitPoint;


    private void Start()
    {
        plane = new GameObject().transform;
    }


    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.GetComponent<Slice>() != null)
        {
            //entryPoint = collision.transform.localPosition;
            entryPoint = transform.position;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<Slice>() != null)
        {
            //exitPoint = collision.transform.localPosition;
            exitPoint = transform.position;



            //this part interacts with OpenFracture
            var slicer = collision.gameObject.GetComponent<Slice>();
            plane.forward = (exitPoint - entryPoint);
            var sliceNormal = plane.up;
            var sliceOrigin = Vector3.Lerp(entryPoint, exitPoint, .5f);

            slicer.ComputeSlice(sliceNormal, sliceOrigin);
        }
    }
}
