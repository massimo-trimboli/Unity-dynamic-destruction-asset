using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetScript : MonoBehaviour
{
    public static bool playerSpawned = false;

    public GameObject player;
    public GameObject playerNoCut;

    public GameObject button;

    static GameObject activePlayer;
    static string activePlayerName;

    static Vector3 playerPos;
    static Quaternion playerRot;


    void Start()
    {
        activePlayer = GameObject.Find(activePlayerName);

        if (!playerSpawned)
        {
            //default on scene start
            activePlayer = player;
            //activePlayer = playerNoCut;
            activePlayerName = activePlayer.name;
            playerPos = activePlayer.transform.position;
            playerRot = activePlayer.transform.localRotation;

            setPlayer();
        }
        else
        {
            setPlayer();
        }

        //make sure timescale correct cause this can be called from pause
        Time.timeScale = 1;

        playerSpawned = true;
    }

    public void resetScene()
    {
        //get active player info
        if (player.active)
        {
            activePlayer = player;
        }
        else
        {
            activePlayer = playerNoCut;
        }
        activePlayerName = activePlayer.name;
        playerPos = activePlayer.transform.position;
        playerRot = activePlayer.transform.rotation;

        //reset scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void setPlayer()
    {
        // activate only active player
        player.SetActive(false);
        playerNoCut.SetActive(false);

        activePlayer.SetActive(true);

        //set transform
        activePlayer.transform.position = playerPos;
        activePlayer.transform.rotation = playerRot;
    }
}
