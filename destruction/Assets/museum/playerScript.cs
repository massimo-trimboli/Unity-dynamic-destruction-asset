using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public float speed;
    public float lookSensetivityX = 1;
    public float lookSensetivityY = 1;
    public GameObject camera;
    public GameObject canvas;
    public TMPro.TMP_InputField lookFieldX;
    public TMPro.TMP_InputField lookFieldY;

    bool canLook = true;
    bool paused = false;


    private void Start()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        if(!paused)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                canLook = false;
            else
                canLook = true;

            move();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
        }
    }

    void move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // movement
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right;
        }
        rb.velocity = movement.normalized * speed;


        // look
        if (canLook )
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSensetivityX, 0);

            float camRotate = camera.transform.localEulerAngles.x;
            camRotate -= Input.GetAxis("Mouse Y") * lookSensetivityY;
            Mathf.Clamp(camRotate, -80, 80);
            camera.transform.localEulerAngles = new Vector3(camRotate, 0, 0);
        }
    }

    public void pause()
    {
        if (!paused)
        {
            //get values
            lookFieldX.text = lookSensetivityX.ToString();
            lookFieldY.text = lookSensetivityY.ToString();

            //pause
            Time.timeScale = 0;
            canvas.SetActive(true);
            paused = true;
        }
        else
        {
            //applpy values
            lookSensetivityX = float.Parse(lookFieldX.text);
            lookSensetivityY = float.Parse(lookFieldY.text);

            if(lookSensetivityX < 0)
                lookSensetivityX *= -1;
            if (lookSensetivityY < 0)
                lookSensetivityY *= -1;
            if (lookSensetivityX == 0)
                lookSensetivityX = 0.1f;
            if (lookSensetivityY == 0)
                lookSensetivityY = 0.1f;

            //un pause
            Time.timeScale = 1;
            canvas.SetActive(false);
            paused = false;
        }
    }
}
