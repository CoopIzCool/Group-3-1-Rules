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
    #endregion Fields

    // Update is called once per frame
    void Update()
    {
        if (grabbedObject != null)
        {
            //grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            Debug.Log(grabbedScreenPos.z);
            grabbedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, grabbedScreenPos.z + zDepth));
            float clampedX = Mathf.Clamp(grabbedObject.transform.position.x, xBound * -1, xBound);
            float clampedY = Mathf.Clamp(grabbedObject.transform.position.y, -1.5f, 3);
            float clampedZ = Mathf.Clamp(grabbedObject.transform.position.z, zBound * -1, zBound);
            grabbedObject.transform.position = new Vector3(clampedX, clampedY, clampedZ);
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                grabbedObject = null;
                cameraRotationScript.ItemIsHeld = false;
                zDepth = 0;
            }
            if (Input.mouseScrollDelta.y > 0.0f )
            {
                Debug.Log("Wee");
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
                hitObject.GetComponent<MeshRenderer>().material = materialTest;
            }
            if (hitObject.tag.Equals("Interactable"))
            {
                //Debug.Log("It should work");
                grabbedObject = hitObject;
                hitObject.GetComponent<MeshRenderer>().material = materialTest;
                cameraRotationScript.ItemIsHeld = true;
                grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            }
        }

    }
}
