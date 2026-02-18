using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voxelDestructionScript : MonoBehaviour
{
    public bool isParent = false;

    public List<GameObject> cubes;
    Dictionary<GameObject, List<GameObject>> neighborDict = new Dictionary<GameObject, List<GameObject>>();
    List<Vector2Int> jointList = new List<Vector2Int>();

    float destructBreakForce;
    bool useBreakForce;


    void Start()
    {
        //only want to execute this once so only run on parent and not individual cubes
        if(isParent)
        {
            destructBreakForce = GetComponent<voxelGridScript>().breakForce;
            useBreakForce = GetComponent<voxelGridScript>().useBreakforce;

            getneighbors();
            getJoints();
            setJoints();
        }
        else
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = !transform.parent.GetComponent<voxelGridScript>().hasRigidBody;
        }
    }



    void getneighbors()
    {
        foreach (GameObject cube in cubes)
        {
            Collider[] neighbors =
                    Physics.OverlapBox(
                        cube.transform.position,
                        cube.GetComponent<MeshRenderer>().bounds.size,
                        cube.transform.rotation);

            List<GameObject> list = new List<GameObject>();
            foreach (Collider col in neighbors)
            {
                if (col.gameObject.GetComponent<voxelDestructionScript>() != null &&
                    col.gameObject.GetComponent<voxelDestructionScript>().isParent == false)
                {
                    list.Add(col.gameObject);
                }
            }
            neighborDict.Add(cube, list);
        }
    }

    void getJoints()
    {
        List<Vector2Int> _tempList = new List<Vector2Int>();

        //fill list
        for (int i = 0; i < cubes.Count; i++)
        {
            List<GameObject> neighbors = neighborDict[cubes[i]];
            for (int j=0; j< neighbors.Count; j++)
            {
                //create joint
                Vector2Int joint = new Vector2Int(i, j);

                //check if its already in the list
                bool isInList = false;
                foreach(Vector2Int jointInList in _tempList)
                {
                    if(joint == jointInList)
                    {
                        isInList = true;
                    }
                }
                //if not add to list
                if(!isInList)
                    _tempList.Add(joint);
            }
        }

        //clean up list
        //print(_tempList.Count);
        List<Vector2Int> _tempList2 = new List<Vector2Int>();
        foreach (Vector2Int joint in _tempList)
        {
            bool isInList = false;
            foreach (Vector2Int joint2 in _tempList2)
            {
                Vector2Int inverse = new Vector2Int(joint2.y, joint2.x);
                if (joint == inverse)
                {
                    isInList = true;
                    //print("double found");
                }
                else if(joint.x == joint.y)
                {
                    isInList = true;
                    //print("is self");
                }
            }
            if (!isInList)
            {
                _tempList2.Add(joint);
            }
        }
        //print(_tempList2.Count);

        //assign our cleaned up list
        jointList = _tempList2;
    }

    void setJoints()
    {
        foreach (Vector2Int v2i in jointList)
        {
            GameObject obj = cubes[v2i.x];
            GameObject target = neighborDict[obj][v2i.y];

            if(obj != target)
            {
                FixedJoint joint = obj.AddComponent<FixedJoint>();
                joint.connectedBody = target.GetComponent<Rigidbody>();
                
                if(useBreakForce)
                {
                    joint.breakForce = destructBreakForce;
                    joint.breakTorque = destructBreakForce;
                }
            }
        }
    }
}
