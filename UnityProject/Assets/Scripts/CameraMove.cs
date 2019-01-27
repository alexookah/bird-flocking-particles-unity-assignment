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

    public bool isLeftButtonPressed = false;
    public bool isRightButtonPressed = false;
    public bool isDownButtonPressed = false;
    public bool isUpButtonPressed = false;

    public bool isSpaceButtonPressed = false;
    public bool isShiftButtonPressed = false;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveDown();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            MoveUp();
        }


        if (Input.GetKey(KeyCode.Space))
        {
            MoveForward();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            MoveBack();
        }


        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            UnlockCameraMouse();
        }

        UpdateMessageAndDissapear();

        if (movingByMouse)
        {
            MoveCameraByMouse();
        }

        if (isLeftButtonPressed)
        {
            MoveLeft();
        }

        if (isRightButtonPressed)
        {
            MoveRight();
        }

        if (isUpButtonPressed)
        {
            MoveUp();
        }

        if (isDownButtonPressed)
        {
            MoveDown();
        }

        if (isSpaceButtonPressed)
        {
            MoveForward();
        }

        if (isShiftButtonPressed)
        {
            MoveBack();
        }
    }

    //LEFT
    public void onPressLeft()
    {
        isLeftButtonPressed = true;
    }

    public void onReleaseLeft()
    {
        isLeftButtonPressed = false;
    }

    //RIGHT
    public void onPressRight()
    {
        isRightButtonPressed = true;
    }

    public void onReleaseRight()
    {
        isRightButtonPressed = false;
    }

    //DOWN
    public void onPressDown()
    {
        isDownButtonPressed = true;
    }

    public void onReleaseDown()
    {
        isDownButtonPressed = false;
    }

    //UP
    public void onPressUp()
    {
        isUpButtonPressed = true;
    }

    public void onReleaseUp()
    {
        isUpButtonPressed = false;
    }

    //SPACE
    public void onPressSpace()
    {
        isSpaceButtonPressed = true;
    }

    public void onReleaseSpace()
    {
        isSpaceButtonPressed = false;
    }

    //SHIFT
    public void onPressShift()
    {
        isShiftButtonPressed = true;
    }

    public void onReleaseShift()
    {
        isShiftButtonPressed = false;
    }


    public void MoveRight() 
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }

    public void MoveLeft()
    {
        transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
    }

    public void MoveUp()
    {
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
    }

    public void MoveDown()
    {
        transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
    }

    public void MoveForward()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }

    public void MoveBack()
    {
        transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    }

    public void UnlockCameraMouse()
    {
        movingByMouse = !movingByMouse;
        SetMessage("Mouse Camera is: " + movingByMouse);
        ShowMessage();
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
