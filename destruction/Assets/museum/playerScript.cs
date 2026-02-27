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

    bool holdingKnife = false;
    bool paused = false;

    [Space(10)]
    public GameObject knife;
    public Transform knifeDefaultPos;
    public Transform knifeHoldPos;
    public float knifeMoveSpeed = 0.05f;
    public float knifeRotateSpeed = 1.5f;
    Vector3 knifePos;


    private void Start()
    {
        canvas.SetActive(false);
        knifePos = knifeHoldPos.position;
    }

    void Update()
    {
        if (!paused)
        {
            move();
            manipulateKnife();
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
        Vector3 movement = new Vector3(0, rb.velocity.y, 0);
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
        if (!holdingKnife)
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


    void manipulateKnife()
    {
        //pick up knife
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            holdingKnife = true;
            knifePos = knifeHoldPos.localPosition;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holdingKnife = false;
        }


        if (holdingKnife)
        {
            Vector3 swing = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            swing.x /= 1.95f;
            swing.y /= 1.95f;
            knifePos += swing;

            float x = knifePos.x;
            float y = knifePos.y;
            x = Mathf.Clamp(x, -2, 3);
            y = Mathf.Clamp (y, -1.8f, .8f);

            knifePos = new Vector3(x, y, knifePos.z);
            //print(knifePos);
        }
        moveKnife();
        
    }
    void moveKnife()
    {
        if (holdingKnife)
        {
            knife.transform.localPosition = Vector3.MoveTowards(
                knife.transform.localPosition,
                knifePos,
                knifeMoveSpeed
            );
            knife.transform.localEulerAngles = Vector3.MoveTowards(
                knife.transform.localEulerAngles,
                knifeHoldPos.localEulerAngles,
                knifeRotateSpeed
            );
        }
        else
        {
            knife.transform.localPosition = Vector3.MoveTowards(
                knife.transform.localPosition,
                knifeDefaultPos.localPosition,
                knifeMoveSpeed
            );
            knife.transform.localEulerAngles = Vector3.MoveTowards(
                knife.transform.localEulerAngles,
                knifeDefaultPos.localEulerAngles,
                knifeRotateSpeed
            );
        }
    }
}
