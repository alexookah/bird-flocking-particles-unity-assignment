using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 50.0f;


    public float speedH = 5.0f;
    public float speedV = 5.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public bool movingByMouse = false;
    private string message;
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    private bool displayMessage = true;

    private float displayTime = 3.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }


        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));

        }


        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            movingByMouse = !movingByMouse;
            SetMessage("Mouse Camera is: " + movingByMouse);
            ShowMessage();
        }

        UpdateMessageAndDissapear();

        if (movingByMouse)
        {
            MoveCameraByMouse();
        }

    }

    public void ShowMessage()
    {
        displayTime = 3.0f;

    }

    public void SetMessage(string msg)
    {
        message = msg;
    }

    void UpdateMessageAndDissapear()
    {
        displayTime -= Time.deltaTime;
        if (displayTime <= 0.0f)
        {
            displayMessage = false;
        }
        else
        {
            displayMessage = true;
        }

    }

    void MoveCameraByMouse()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void OnGUI()
    {
        if (displayMessage)
        {
            guiStyle.fontSize = 50;
            GUI.Label(new Rect(20.0f, Screen.height - 60.0f, 200f, 200f),message, guiStyle);
        }
    }
}
