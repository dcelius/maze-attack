using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour {

    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    private float mouseScroll;
    public float thrust;
    public MainMenu mainMenu;

    void Start()
    {
    }

    void Update()
    {
        if (mainMenu.getIsPlaying())
        {
            /*
            Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
            Converted to C# 27-02-13 - no credit wanted.
            */
            if (Input.GetMouseButtonDown(1))
            {
                lastMouse = Input.mousePosition; 

            }
            if (Time.timeScale > 0 && Input.GetMouseButton(1))
            {
                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
                transform.eulerAngles = lastMouse;
                lastMouse = Input.mousePosition;
            }

            Vector3 p = GetBaseInput();
            p = p * thrust;
            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;

            mouseScroll = Input.mouseScrollDelta.y;
            if (mouseScroll != 0)
            {   //Code is written by JelleWho https://github.com/jellewie
                float r = mouseScroll * 5;                                          // Distance to move
                float posX = transform.eulerAngles.x + 90;                          // Get up and down
                float posY = -1 * (transform.eulerAngles.y - 90);                   // Get left to right
                posX = posX / 180 * Mathf.PI;                                       // Convert from degrees to radians
                posY = posY / 180 * Mathf.PI;                                       // ^
                float x = r * Mathf.Sin(posX) * Mathf.Cos(posY);                    // Calculate new coords
                float z = r * Mathf.Sin(posX) * Mathf.Sin(posY);                    // ^
                float y = r * Mathf.Cos(posX);                                      // ^
                float newX = Mathf.Clamp(transform.position.x + x, 0, 1000);        // Create new values - clamped to play area
                float newY = Mathf.Clamp(transform.position.y + y, 0, 1000);        // ^
                float newZ = Mathf.Clamp(transform.position.z + z, 0, 1000);        // ^
                transform.position = new Vector3(newX, newY, newZ);                 //Move the main camera

                thrust += -mouseScroll * 2;                                         // Adjust thrust
            }
            else if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
            { // If not trying to ascend or descend
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }

        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }

}