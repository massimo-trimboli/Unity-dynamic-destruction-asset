using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class buttonscript : MonoBehaviour
{
    public Collider button;
    public Collider buttonPushedIn;
    public Collider buttonBox;
    public Collider border;
    public Collider[] knives;

    public UnityEvent onButtonPress;

    public GameObject[] bunnies;
    public GameObject[] objsToDestroy;
    public GameObject objToSpawn;
    public Rigidbody car;
    public GameObject wall;

    bool shitHappenedOnce = false;


    void Start()
    {
        Physics.IgnoreCollision(button, buttonBox, true);
        Physics.IgnoreCollision(buttonPushedIn, buttonBox, true);
        foreach (Collider knife in knives){
            Physics.IgnoreCollision(knife, border, true);
        }

        //launchCar();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider == buttonPushedIn)
        {
            onButtonPress.Invoke();
        }
    }


    public void resetBunnies()
    {
        foreach (GameObject bunny in bunnies)
        {
            bunny.transform.position = bunny.transform.position + new Vector3(0, 5, 0);
        }
    }

    public void startPart3()
    {
        if (!shitHappenedOnce)
        {
            deletePast();
            spawnNew();
            launchCar();

            shitHappenedOnce = true;
        }
    }
    void deletePast()
    {
        foreach(GameObject obj in objsToDestroy)
        {
            Destroy(obj);
        }
    }
    void launchCar()
    {
        wall.GetComponent<voxelScript>().callVoxelise();
        car.AddForce(new Vector3(1000,0,0), ForceMode.Impulse);
    }
    void spawnNew()
    {
        objToSpawn.SetActive(true);
    }

}
