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
    #endregion Fields

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedObject != null)
        {
            grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            grabbedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, grabbedScreenPos.z));
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                grabbedObject = null;
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
            }
        }

    }
}
