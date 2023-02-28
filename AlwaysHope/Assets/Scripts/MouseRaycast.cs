using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    #region Fields
    Ray mouseRay;
    RaycastHit hitInfo;
    [SerializeField]
    Material materialTest;
    public GameObject grabbedObject;
    public Vector3 grabbedScreenPos;
    private float zDepth = 0;
    private float xBound = 4.5f;
    private float zBound = 4.5f;
    [SerializeField]
    CameraFixedRotation cameraRotationScript;
    [SerializeField] public int interactableSolvedCount = 0;
    [SerializeField] public int interactableSolvedGoal = 1;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private AudioClip victoryClip;
    #endregion Fields
    public AudioClip VictoryClip { get { return victoryClip; } }

    // Update is called once per frame
    void Update()
    {
        if (grabbedObject != null)
        {
            //grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            grabbedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, grabbedScreenPos.z + zDepth));

            //Clamp the position to prevent the object from falling out of bounds
            float clampedX = Mathf.Clamp(grabbedObject.transform.position.x, xBound * -1, xBound);
            float clampedY = Mathf.Clamp(grabbedObject.transform.position.y, .5f, 5);
            float clampedZ = Mathf.Clamp(grabbedObject.transform.position.z, zBound * -1, zBound);
            grabbedObject.transform.position = new Vector3(clampedX, clampedY, clampedZ);

            //release the object if the mouse is let go
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                grabbedObject = null;
                cameraRotationScript.ItemIsHeld = false;
                zDepth = 0;
            }

            //Scroll depth of the zPosition 
            if (Input.mouseScrollDelta.y > 0.0f )
            {
                zDepth += 0.2f;
            }
            else if (Input.mouseScrollDelta.y < 0.0f )
            {
                zDepth -= 0.2f;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                RaycastTest();
        }
        if(interactableSolvedCount == interactableSolvedGoal)
        {
            endScreen.SetActive(true);
            //Time.timeScale = 0f;
        }

    }

    private void RaycastTest()
    {
        Debug.Log("Testing");
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay, out hitInfo))
        {
            Debug.Log("Hit something");
            GameObject hitObject = hitInfo.collider.gameObject;
            Debug.Log(hitObject.name);
            if (hitObject.tag.Equals("Test"))
            {
                Debug.Log("It should work");
                //hitObject.GetComponent<MeshRenderer>().material = materialTest;
            }
            if (hitObject.tag.Equals("Interactable"))
            {
                //Debug.Log("It should work");
                grabbedObject = hitObject;
                //hitObject.GetComponent<MeshRenderer>().material = materialTest;
                //bool to flip scroll wheel functionallity
                cameraRotationScript.ItemIsHeld = true;
                grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            }
        }

    }
}
