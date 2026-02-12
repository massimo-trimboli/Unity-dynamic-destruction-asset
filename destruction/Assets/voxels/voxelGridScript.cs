using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(MeshCollider))]
public class voxelGridScript : MonoBehaviour
{
    public float sizeOfVoxels = 1;

    public enum voxeliseOptions {combine, continuous, destructible }
    public voxeliseOptions voxeliseOption;

    public bool hasRigidBody;



    void Start()
    {
        GetComponent<MeshCollider>().convex = false;
        voxelise(gameObject, sizeOfVoxels, voxeliseOption);


        Invoke("addRb", 0.25f);
    }
    void addRb()
    {
        if(hasRigidBody) gameObject.AddComponent<Rigidbody>();
    }


    public static void voxelise(GameObject targetObject, float sizeOfVoxels, voxeliseOptions voxeliseOption)
    {
        string option = voxeliseOption.ToString();


        Bounds bound = targetObject.GetComponent<MeshRenderer>().bounds;

        List<GameObject> cubeGrid = voxelCreateGrid(bound, sizeOfVoxels);
        voxelFillGrid(targetObject, cubeGrid, option);
    }


    static List<GameObject> voxelCreateGrid(Bounds bound, float sizeOfVoxels)
    {
        List<GameObject> _tempList = new List<GameObject>();


        // create a cube for reference
        GameObject refCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 refCubeSize = new Vector3(sizeOfVoxels, sizeOfVoxels, sizeOfVoxels);
        refCube.transform.localScale = refCubeSize;

        int voxAmountX = (int)Math.Ceiling(bound.size.x / refCubeSize.x);
        int voxAmountY = (int)Math.Ceiling(bound.size.y / refCubeSize.y);
        int voxAmountZ = (int)Math.Ceiling(bound.size.z / refCubeSize.z);


        //y third level
        // for better understanding, this loop should be read from the bottom up
        for (int y = 0; y < voxAmountY; y++)
        {
            Vector3 offsetY = new Vector3(0, refCubeSize.y * y, 0);
            Vector3 startY = bound.center + new Vector3(-bound.extents.x + refCubeSize.x/2,
                -bound.extents.y + refCubeSize.y/2,
                -bound.extents.z + refCubeSize.z/2) + offsetY;

            //x secondary level
            for (int x = 0; x < voxAmountX; x++)
            {
                Vector3 offsetX = new Vector3(refCubeSize.x * x, 0, 0);
                Vector3 startX = startY + offsetX;

                //z base level
                for (int z = 0; z < voxAmountZ; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = new Vector3(sizeOfVoxels, sizeOfVoxels, sizeOfVoxels);

                    Vector3 startZ = startX;
                    Vector3 offset = new Vector3(0, 0, refCubeSize.z * z);
                    cube.transform.position = startZ + offset;

                    cube.name = $"gridBlock_{_tempList.Count.ToString()}";
                    _tempList.Add(cube);
                }
            }
        }
        Destroy(refCube);

        return _tempList;
    }

    static void voxelFillGrid(GameObject target, List<GameObject> grid, string option)
    {
        List<GameObject> _tempList = new List<GameObject>();


        foreach (GameObject block in grid)
        {
            //detect if gridblock contains target
            Collider[] collisions =
            Physics.OverlapBox(block.transform.position,
                block.GetComponent<MeshRenderer>().bounds.extents,
                block.transform.rotation);
            bool isTouchModel = false;
            foreach (Collider collision in collisions)
            {
                if(collision == target.GetComponent<Collider>()) isTouchModel = true;
            }

            if (isTouchModel)
            {
                _tempList.Add(block);
            }
            else
            {
                Destroy(block);
            }
        }

        if (option == "combine")
        {
            target.GetComponent<MeshFilter>().mesh.Clear();
            target.GetComponent<MeshCollider>().convex = true;
            foreach (GameObject block in _tempList)
            {
                block.transform.parent = target.transform;
                Destroy(block.GetComponent<Collider>());
            }
        }
    }
}
