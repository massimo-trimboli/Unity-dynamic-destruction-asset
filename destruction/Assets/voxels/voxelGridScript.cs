using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voxelGridScript : MonoBehaviour
{
    public float voxelSize;



    void Start()
    {
        Bounds bound = GetComponent<MeshRenderer>().bounds;

        //* ///////////////////////////////////////////////////////////////////////////////////
        //this whole part is just for visualisation
        GameObject boundingBox = new GameObject("boundingBox");
        boundingBox.AddComponent<BoxCollider>();
        boundingBox.layer = 2; // ignore raycasts

        boundingBox.transform.position = bound.center;
        boundingBox.transform.localScale = bound.size;
        ///////////////////////////////////////////////////////////////////////////////////////
        // */

        createGrid(bound, voxelSize);
    }


    public /*static*/ void createGrid(Bounds bound, float voxelSize)
    {
        // create a cube for reference
        GameObject refCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 refCubeSize = new Vector3(voxelSize, voxelSize, voxelSize);
        refCube.transform.localScale = refCubeSize;

        int voxAmountX = (int)Math.Ceiling(bound.size.x / refCubeSize.x);
        int voxAmountY = (int)Math.Ceiling(bound.size.y / refCubeSize.y);
        int voxAmountZ = (int)Math.Ceiling(bound.size.z / refCubeSize.z);


        //y third level
        // for better understanding, this loop should be read from the bottom up
        for (int y = 0; y < voxAmountY; y++)
        {
            Vector3 offsetY = new Vector3(0, refCubeSize.y * y, 0);
            Vector3 startY = bound.center
                            + new Vector3(-bound.extents.x + refCubeSize.x/2,
                                          -bound.extents.y + refCubeSize.y/2,
                                          -bound.extents.z + refCubeSize.z/2)
                            + offsetY;

            //x secondary level
            for (int x = 0; x < voxAmountX; x++)
            {
                Vector3 offsetX = new Vector3(refCubeSize.x * x, 0, 0);
                Vector3 startX = startY + offsetX;

                //z base level
                for (int z = 0; z < voxAmountZ; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

                    Vector3 startZ = startX;
                    Vector3 offset = new Vector3(0, 0, refCubeSize.z * z);
                    cube.transform.position = startZ + offset;

                    //cube.GetComponent<MeshRenderer>().enabled = false;
                    cube.transform.parent = gameObject.transform;
                }
            }
        }
        Destroy(refCube);
    }
}
