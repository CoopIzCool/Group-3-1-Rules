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
    #endregion Fields
    // Start is called before the first frame update
    void Start()
    {
        radius = 12.0f;
        yPos = transform.position.y;
        counter = 180;
        xRotateSensitivity = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //increments counter;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            counter += 0.2f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            counter -= 0.2f;
        }

        //camera Zoom
        if (Input.mouseScrollDelta.y > 0.0f && GetComponent<Camera>().fieldOfView > 20.0f)
        {
            GetComponent<Camera>().fieldOfView -= 0.5f;
            xRotateSensitivity -= 0.02f;
        }
        else if (Input.mouseScrollDelta.y < 0.0f && GetComponent<Camera>().fieldOfView < 90.0f)
        {
            GetComponent<Camera>().fieldOfView += 0.5f;
            xRotateSensitivity += 0.02f;
        }
        //changes cameras horizantal view
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            xRotate -= 0.5f;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            xRotate += 0.5f;
        }

        //reset counter for easy calculations
        if (counter >= 360.0f || counter <= -360.0f)
        {
            counter = 0.0f;
        }
        //convert x and y to radians
        float radians = counter * (Mathf.PI / 180.0f);
        float x = Mathf.Sin(radians) * radius;
        float z = Mathf.Cos(radians) * radius;
        //set position and rotation
        transform.position = new Vector3(x, yPos, z);
        transform.rotation = Quaternion.Euler(xRotate * xRotateSensitivity, counter - 180, 0);

    }
}
