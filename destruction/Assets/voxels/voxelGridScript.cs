using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


//[RequireComponent(typeof(MeshCollider))]
public class voxelGridScript : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> gridList;

    
    public enum resMethod { cubeAmount, cubeSize}
    [Header("resolution options")]
    public resMethod resolutionBy = resMethod.cubeAmount;
    public int cubeAmount = 15;
    public enum axis { x, y, z }
    public axis amountAlongAxis;
    public float cubeSize = 1;

    float sizeOfVoxels = 1;
    [Space(20)]

    [Header("miscelaneous options")]
    public bool voxeliseOnStart = true;
    [Tooltip("if object has a rigidbody at start, it doesnt work so if you want to modify properties of rigidbody, edit the 'addRB' method")]
    public bool hasRigidBody;

    public enum voxeliseOptions {combine, explode, destructible }
    [Header("voxelise options")]
    public voxeliseOptions voxeliseOption;
    [Header("voxelise option:explode properties")]
    public float explodeForce = 10;
    [Header("voxelise option:destructible - properties")]
    public bool useBreakforce = true;
    public float breakForce = 250;




    void Start()
    {
        if (voxeliseOnStart)
            callVoxelise();
    }
    public void callVoxelise()
    {
        if (GetComponent<MeshCollider>() == null) gameObject.AddComponent<MeshCollider>();

        setSizeOfVoxels();

        voxelise(gameObject, sizeOfVoxels, voxeliseOption, ref gridList);
        Invoke("addRb", .25f);
    }
    void addRb()
    {
        if(hasRigidBody)
        {
            if(voxeliseOption != voxeliseOptions.destructible)
            {
                gameObject.AddComponent<Rigidbody>();
            }
        }
    }
    void setSizeOfVoxels()
    {
        if(resolutionBy == resMethod.cubeAmount)
        {
            if(amountAlongAxis == axis.x)
            {
                Bounds bund = GetComponent<MeshRenderer>().bounds;
                sizeOfVoxels = bund.size.x / cubeAmount;
            }
            else if (amountAlongAxis == axis.y)
            {
                Bounds bund = GetComponent<MeshRenderer>().bounds;
                sizeOfVoxels = bund.size.y / cubeAmount;
            }
            else if (amountAlongAxis == axis.z)
            {
                Bounds bund = GetComponent<MeshRenderer>().bounds;
                sizeOfVoxels = bund.size.z / cubeAmount;
            }
        }
        else if (resolutionBy == resMethod.cubeSize)
        {
            sizeOfVoxels = cubeSize;
        }
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
            float explodeForce = target.GetComponent<voxelGridScript>().explodeForce;

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
                bRB.velocity = forceDirection.normalized * explodeForce;
            }
        }
        else if (option == "destructible")
        {
            target.GetComponent<MeshFilter>().mesh.Clear();
            //target.GetComponent<MeshCollider>().convex = true;
            Destroy(target.GetComponent<MeshCollider>());
            foreach (GameObject block in voxels)
            {
                block.transform.parent = target.transform;

                voxelDestructionScript childScript = block.AddComponent<voxelDestructionScript>();
                //childScript.cubes = voxels;
            }
            voxelDestructionScript parentScript = target.AddComponent<voxelDestructionScript>();
            parentScript.cubes = voxels;
            parentScript.isParent = true;
        }

        //assign list
        grid = voxels;
    }

    static void explode(GameObject target, List<GameObject> voxels)
    {
        float explodeForce = target.GetComponent<voxelGridScript>().explodeForce;

        Destroy(target);

        Vector3 centerOM = Vector3.zero;
        foreach (GameObject block in voxels)
        {
            block.transform.parent = null;
            centerOM += block.transform.position;
        }
        centerOM = centerOM / voxels.Count;

        // have blocks explode from center
        foreach (GameObject block in voxels)
        {
            Rigidbody bRB = block.AddComponent<Rigidbody>();
            Vector3 forceDirection = block.transform.position - centerOM;
            bRB.velocity = forceDirection.normalized * explodeForce;
        }
    }
}
