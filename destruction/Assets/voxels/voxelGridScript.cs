using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


[RequireComponent(typeof(MeshCollider))]
public class voxelGridScript : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> gridList;


    [Tooltip("smaller values make for more acurate results but worse performance")]
    public float sizeOfVoxels = 1;
    public bool voxeliseOnStart;
    [Tooltip("if object has a rigidbody at start, it doesnt work so if you want to modify properties of rigidbody, edit the 'addRB' method")]
    public bool hasRigidBody;

    public enum voxeliseOptions {combine, explode, destructible }
    public voxeliseOptions voxeliseOption;
    public float explodeForce = 0;
    static float explodeForceStatic;




    void Start()
    {
        if(voxeliseOnStart)
            callVoxelise();
    }
    public void callVoxelise()
    {
        if (voxeliseOption == voxeliseOptions.explode)
            explodeForceStatic = explodeForce;

        voxelise(gameObject, sizeOfVoxels, voxeliseOption, ref gridList);
        Invoke("addRb", 0.25f);
    }
    void addRb()
    {
        if(hasRigidBody) gameObject.AddComponent<Rigidbody>();
    }



    static void voxelise(GameObject targetObject, float sizeOfVoxels, voxeliseOptions voxeliseOption, ref List<GameObject> gridList)
    {
        //collider can be convex for physics purposes but a convex mesh will mess with generation of voxels
        targetObject.GetComponent<MeshCollider>().convex = false;

        //create the grid
        Bounds bound = targetObject.GetComponent<MeshRenderer>().bounds;
        gridList = voxelCreateGrid(bound, sizeOfVoxels);

        //loop through grid
        string option = voxeliseOption.ToString();
        voxelFillGrid(targetObject, ref gridList, option);
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

    static void voxelFillGrid(GameObject target, ref List<GameObject> grid, string option)
    {
        List<GameObject> voxels = new List<GameObject>();


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
                voxels.Add(block);
            }
            else
            {
                Destroy(block);
            }
        }

        if (option == "combine")
        {
            target.GetComponent<MeshFilter>().mesh.Clear();
            //target.GetComponent<MeshCollider>().convex = true;
            Destroy(target.GetComponent<MeshCollider>());
            foreach (GameObject block in voxels)
            {
                block.transform.parent = target.transform;
                //Destroy(block.GetComponent<Collider>());
            }
        }
        else if (option == "explode")
        {
            Destroy(target);

            Vector3 centerOM = Vector3.zero;
            foreach (GameObject block in voxels)
            {
                centerOM += block.transform.position;
            }
            centerOM = centerOM / voxels.Count;

            // have blocks explode from center
            foreach (GameObject block in voxels)
            {
                Rigidbody bRB = block.AddComponent<Rigidbody>();
                Vector3 forceDirection = block.transform.position - centerOM;
                bRB.velocity = forceDirection.normalized * explodeForceStatic;
            }
        }

        //assign list
        grid = voxels;
    }
}
