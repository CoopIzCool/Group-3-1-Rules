using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Code developed by Ryan Cooper in 2020
/// </summary>
public class CameraFixedRotation : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float radius;
    private float counter;
    private float yPos;
    private float xRotate;
    private float xRotateSensitivity;
    private bool itemHeld;
    [SerializeField]
    private Transform centerPoint;

    #endregion Fields

    #region Properties
    public bool ItemIsHeld
    {
        set { itemHeld = value; }
    }
    #endregion Properties
    // Start is called before the first frame update
    void Start()
    {
        radius = 12.0f;
        yPos = transform.position.y;
        counter = 180;
        xRotate = Mathf.PI / 4;
        xRotateSensitivity = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //increments counter;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            counter += 20.0f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            counter -= 20.0f * Time.deltaTime;
        }

        //If the player is not holding an item
        if(!itemHeld)
        {
            //camera Zoom
            if (Input.mouseScrollDelta.y > 0.0f && GetComponent<Camera>().fieldOfView > 20.0f)
            {
                GetComponent<Camera>().fieldOfView -= 175.0f * Time.deltaTime;
                xRotateSensitivity -= 5.00f * Time.deltaTime;
            }
            else if (Input.mouseScrollDelta.y < 0.0f && GetComponent<Camera>().fieldOfView < 90.0f)
            {
                GetComponent<Camera>().fieldOfView += 175.0f * Time.deltaTime;
                xRotateSensitivity += 5.00f * Time.deltaTime;
            }

        }
        //changes cameras horizantal view
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            xRotate -= 0.5f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            xRotate += 0.5f * Time.deltaTime;
        }

        //reset counter for easy calculations
        
        if (counter >= 360.0f || counter <= -360.0f)
        {
            counter = 0.0f;
        }
        //Clamp XRotation to prevent Axis Flipping
        xRotate = Mathf.Clamp(xRotate, 0.1f, Mathf.PI/2);
        //Debug.Log(xRotate);
        //convert x and y to radians
        float radians = counter * (Mathf.PI / 180.0f);
        float x = Mathf.Sin(radians) * Mathf.Sin(xRotate) * radius;
        float y = Mathf.Cos(xRotate) * radius;
        float z = Mathf.Cos(radians) * Mathf.Sin(xRotate) * radius;
        //set position and rotation
        transform.position = new Vector3(x, y, z);
        transform.LookAt(centerPoint);
    }
}
