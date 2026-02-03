using GK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class softBuddy : MonoBehaviour
{
    //mesh info
    MeshFilter ogMeshFilter;
    Mesh mesh2;

    List<Vector3> vertexList = new List<Vector3>();
    List<Vector3> normalList = new List<Vector3>();
    int[] triArray;
    Dictionary<int,int> edgeDictionary = new Dictionary<int,int>();
        //for convex hull optimisation
    List<Vector3> vertexListOptimised = new List<Vector3>();
    List<Vector3> normalListOptimsed = new List<Vector3>();
    List<int> triListOptimsed;


    List<SphereCollider> sphereColliderList = new List<SphereCollider>();
    List<GameObject> physicsVertexList = new List<GameObject>();
    GameObject centerOfMass;


    //public variables
    public bool optimisedReducPoints = true;
    public bool rbUseGravity = true;
    public float colliderRadius = 0.1f;
    public float rbMass = 1;
    public float rbDrag = 1;



    void Awake()
    {
        //assigner mesh / vertex / normals
        ogMeshFilter = GetComponent<MeshFilter>();
        ogMeshFilter.mesh.GetVertices(vertexList);
        ogMeshFilter.mesh.GetNormals(normalList);

        triArray = ogMeshFilter.mesh.triangles;

        // passer de possition locale à position monde
        for ( int i=0; i < vertexList.Count; i++)
        {
            //je comprends pas entierement la ligne de code mais ca va etre pour plus tard
            vertexList[i] = transform.localToWorldMatrix.MultiplyPoint3x4(vertexList[i]);
        }

        // //////////////////////
        //optimisations utilisant le script ConvexHullCalculator par Eshan Moradi
        // https://github.com/ehsanwwe/Unity-SoftBody-physics/blob/main/Assets/ProSoftBody/ConvexHullCalculator.cs
        if (optimisedReducPoints)
        {
            new ConvexHullCalculator().GenerateHull(
                vertexList,
                false,
                ref vertexListOptimised,
                ref triListOptimsed,
                ref normalListOptimsed
            );
            vertexList = vertexListOptimised;
            normalList = normalListOptimsed;
            triArray = triListOptimsed.ToArray();
        }
        // //////////////////////

        mesh2 = new Mesh();
        mesh2.MarkDynamic();
        mesh2.SetVertices(vertexList);
        mesh2.SetNormals(normalList);
        mesh2.triangles = triArray;
        ogMeshFilter.mesh = mesh2;


        // //////////////////////
        // this is where we pair up mesh vertexes and physics vertexes
        // if the mesh has any duplicated vertexes, we assign them to the same physics point
        var _optimizedVertex = new List<Vector3>();
        for(int i = 0; i< vertexList.Count; i++)
        {
            bool hasPair = false;
            for (int j = 0; j < _optimizedVertex.Count; j++)
            {
                /*if (vertexList[i] == _optimizedVertex[j])
                {
                    hasPair = true;
                    edgeDictionary.Add(i, j);
                    break;
                }*/
            }
            if (!hasPair)
            {
                _optimizedVertex.Add(vertexList[i]);
                edgeDictionary.Add(i, _optimizedVertex.Count - 1);
            }
        }
        // //////////////////////


        //create physicsPoint for each vertex
        foreach (var vert in _optimizedVertex)
        {
            //creer objet pour vertex
            var tempObj = new GameObject("point "+ _optimizedVertex.IndexOf(vert));
            
            tempObj.transform.parent = transform;
            tempObj.transform.position = vert;
            
            //rajouter collider
            SphereCollider collider = tempObj.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;

            sphereColliderList.Add(collider);
            
            //rajouter rigidbody
            Rigidbody rb = tempObj.AddComponent<Rigidbody>();
            rb.mass = rbMass / _optimizedVertex.Count;
            rb.drag = rbDrag;
            rb.useGravity = rbUseGravity;


            physicsVertexList.Add(tempObj);
        }


        //center of mass
        Vector3 TempCenterOM = new Vector3(0,0,0);
        //position moyenne
        foreach(var vert in vertexList)
        {
            TempCenterOM += vert;
        }
        TempCenterOM = TempCenterOM / vertexList.Count;
        {
            var tempObj = new GameObject("centerOfMass");
            tempObj.transform.parent = transform;
            tempObj.transform.position = TempCenterOM;

            //rajouter collider et rb
            SphereCollider collider = tempObj.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;
            sphereColliderList.Add(collider);
            Rigidbody rb = tempObj.AddComponent<Rigidbody>();
            rb.mass = rbMass / _optimizedVertex.Count;
            rb.drag = rbDrag;
            rb.useGravity = rbUseGravity;

            centerOfMass = tempObj;
        }


        //ignorer collisions entre vertexes
        foreach(var colider1 in sphereColliderList)
        {
            foreach(var collider2 in sphereColliderList)
            {
                Physics.IgnoreCollision(colider1, collider2, true);
            }
        }


        //get les edges
        List<Vector2Int> edgeList = new List<Vector2Int>();

        for(int i = 0; i<triArray.Length; i+=3)
        {
            int point1 = edgeDictionary[triArray[i]];
            int point2 = edgeDictionary[triArray[i+1]];
            int point3 = edgeDictionary[triArray[i+2]];

            //rajouter les 3 cotes du triangle dans la liste
            edgeList.Add(new Vector2Int(point1,point2));
            edgeList.Add(new Vector2Int(point1, point2));
            edgeList.Add(new Vector2Int(point1, point2));
        }
        //enlever les edge en double
        {
            List<Vector2Int> tempList = new List<Vector2Int>();
            foreach(var edge in edgeList)
            {
                bool inList = false;
                foreach(var edge2 in tempList)
                {
                    if (edge == edge2)
                    {
                        inList = true;
                        break;
                    }
                }
                if (!inList)
                {
                    tempList.Add(edge);
                }
            }
            edgeList = tempList;
        }

        //rajouter springs pour chaque edge
        foreach(var edge in edgeList)
        {
            GameObject obj = physicsVertexList[edge.x];
            GameObject target = physicsVertexList[edge.y];

            var joint = obj.AddComponent<SpringJoint>();
            joint.connectedBody = target.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
