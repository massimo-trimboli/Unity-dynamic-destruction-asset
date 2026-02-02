using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class softBuddy : MonoBehaviour
{
    MeshFilter ogMeshFilter;
    Mesh mesh2;

    List<Vector3> vertexList;
    List<Vector3> normalList;
    int[] triArray;

    List<SphereCollider> sphereColliderList;
    List<GameObject> physicsVertexList;
    GameObject centerOfMass;


    
    void Awake()
    {
        //assigner vertex / normals
        vertexList = new List<Vector3>();
        normalList = new List<Vector3>();
        //physicsVertexList = new List<GameObject>();

        ogMeshFilter = GetComponent<MeshFilter>();
        ogMeshFilter.mesh.GetVertices(vertexList);
        ogMeshFilter.mesh.GetNormals(normalList);

        triArray = ogMeshFilter.mesh.triangles;

        // passer de possition locale à position monde
        //je comprends pas entierement la ligne de code mais ca va etre pour plus tard
        for ( int i=0; i < vertexList.Count; i++)
        {
            vertexList[i] = transform.localToWorldMatrix.MultiplyPoint3x4(vertexList[i]);
        }

        // //////////////////////
        // this is where optimisation goes
        // //////////////////////

        mesh2 = new Mesh();
        mesh2.MarkDynamic();
        mesh2.SetVertices(vertexList);
        mesh2.SetNormals(normalList);
        mesh2.triangles = triArray;
        ogMeshFilter.mesh = mesh2;


        // //////////////////////
        // this is where remove duplicate vertex
        // //////////////////////


        //create physicsPoint for each vertex
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
