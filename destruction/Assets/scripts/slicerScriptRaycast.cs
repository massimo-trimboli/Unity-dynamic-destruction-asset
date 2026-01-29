using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slicerScriptRaycast : MonoBehaviour
{
    Transform plane;
    Vector3 entryPoint;
    Vector3 exitPoint;


    void Start()
    {
        plane = new GameObject().transform;
    }

    void Update()
    {
        useRaycast();
    }


    Dictionary<GameObject, Vector3> entryPointsDict = new Dictionary<GameObject, Vector3>();
    void useRaycast()
    {
        //set up overlapbox
        Collider[] hits =
        Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size / 2, transform.rotation);


        HashSet<GameObject> currentHash = new HashSet<GameObject>();

        //put each detected sliceable object in a hashset and put its entry point in a dictionary
        foreach (Collider hit in hits)
        {
            //on enter overlap
            if(hit.gameObject.GetComponent<Slice>() != null)
            {
                //add to hash
                currentHash.Add(hit.gameObject);

                //add to dictionary
                if(!entryPointsDict.ContainsKey(hit.gameObject))
                {
                    entryPointsDict[hit.gameObject] = transform.position;
                }
            }
        }

        //look for exits
        //doing this to itterate through dictionary
        List<GameObject> keys = new List<GameObject>(entryPointsDict.Keys);
        foreach (GameObject obj in keys)
        {
            //if dictionary has this obj but hash doesn't (not overlapping, exited the collider)
            if(!currentHash.Contains(obj))
            {
                //can get entry pos by looking up dictionary
                Vector3 entryPos = entryPointsDict[obj];
                Vector3 exitPos = transform.position;

                //clear dictionary
                entryPointsDict.Remove(obj);

                cut(obj, entryPos, exitPos);
            }
        }
    }


    void cut(GameObject obj, Vector3 entryPos, Vector3 exitPos)
    {
        //this part interacts with OpenFracture
        var slicer = obj.GetComponent<Slice>();
        plane.forward = (exitPos - entryPos);
        var sliceNormal = plane.up;
        var sliceOrigin = Vector3.Lerp(entryPos, exitPos, .5f);

        slicer.ComputeSlice(sliceNormal, sliceOrigin);
    }
}
