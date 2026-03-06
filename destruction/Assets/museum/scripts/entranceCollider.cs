using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entranceCollider : MonoBehaviour
{
    public bool isSideSlice;
    public bool isSideSoft;

    public GameObject player;
    public GameObject playerNoCut;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isSideSoft)
            {
                if (collision.gameObject == player)
                {
                    togglePlayer(player);
                    print("case 1");
                }
            }
            else if (isSideSlice)
            {
                if (collision.gameObject == playerNoCut)
                {
                    togglePlayer(playerNoCut);
                    print("case 2");
                }
            }
            
        }
    }


    void togglePlayer(GameObject playerType)
    {
        // by default player -> player cut
        GameObject playerOld = player;
        GameObject playerNew = playerNoCut;

        if(playerType == playerNoCut)
        {
            //if player walks backwards through museum, inverse the roles 
            playerOld = playerNoCut;
            playerNew = player;
        }
        //look for camera
        Transform camNew = playerNew.transform.Find("Main Camera");
        Transform camOld = playerOld.transform.Find("Main Camera");

        //activate new player and give transform of old
        playerNew.SetActive(true);
        playerNew.transform.position = playerOld.transform.position;
        playerNew.transform.rotation = playerOld.transform.rotation;
        //set cam rotation
        camNew.localRotation = camOld.localRotation;

        //deactivate old
        playerOld.SetActive(false);
    }
}
