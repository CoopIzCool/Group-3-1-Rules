using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float xBoundMin = -8f;
    private float xBoundMax = 8f;
    private float zBound = 6.3f;
    public GameObject fovChangerCanvas;
    [SerializeField]
    CameraFixedRotation cameraRotationScript;
    [SerializeField] public int interactableSolvedCount = 0;
    [SerializeField] public int interactableSolvedGoal = 1;
    [SerializeField] public int optionalInteractableSolvedCount = 0;
    [SerializeField] public int optionalInteractableSolvedGoal = 1;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private Image fovChanger;
    [SerializeField] private GameObject[] requiredInteractables;
    [SerializeField] private GameObject[] optionalInteractables;

    // Inactive timer mechanics
    [SerializeField] private float inactiveTime;
    private float activeTimer;
    private bool mouseActive = true;
    #endregion Fields
    public AudioClip VictoryClip { get { return victoryClip; } }
    public bool MouseActive { get { return mouseActive; } }

    private void Start()
    {
        interactableSolvedGoal = requiredInteractables.Length;
        optionalInteractableSolvedGoal = optionalInteractables.Length;
    }

    // Update is called once per frame
    void Update()
    {
        bool isActive = false;
        if (grabbedObject != null)
        {
            SetActive();
            //grabbedScreenPos = Camera.main.WorldToScreenPoint(grabbedObject.transform.position);
            grabbedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, grabbedScreenPos.z + zDepth));

            //Clamp the position to prevent the object from falling out of bounds
            float clampedX = Mathf.Clamp(grabbedObject.transform.position.x, xBoundMin, xBoundMax);
            Debug.Log(clampedX);
            float clampedY = Mathf.Clamp(grabbedObject.transform.position.y, 0.03f, 5);
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
                zDepth += 0.4f;
            }
            else if (Input.mouseScrollDelta.y < 0.0f )
            {
                zDepth -= 0.4f;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastTest();
                SetActive();
            }
        }
        scaleFOV();
        if(interactableSolvedCount == interactableSolvedGoal)
        {
            fovChangerCanvas.SetActive(false);
            endScreen.SetActive(true);
            //Time.timeScale = 0f;
        }

        if(!isActive && activeTimer < inactiveTime)
        {

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
                if (hitObject.GetComponent<Interactables>().isPreReqMet)
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

    private void scaleFOV()
    {
        float fovScaleX, fovScaleY;
        if ((interactableSolvedGoal - interactableSolvedCount) > 0)
        {
            fovScaleX = (fovChangerCanvas.GetComponent<RectTransform>().rect.width/2) + (fovChanger.rectTransform.sizeDelta.x /  (interactableSolvedGoal - interactableSolvedCount));
            fovScaleY = (fovChangerCanvas.GetComponent<RectTransform>().rect.height/2) + (fovChanger.rectTransform.sizeDelta.y / (interactableSolvedGoal - interactableSolvedCount));
        }
        else
        {
            fovScaleX = fovChangerCanvas.GetComponent<RectTransform>().rect.width;
            fovScaleY = fovChangerCanvas.GetComponent<RectTransform>().rect.height;
        }
        Debug.Log(fovScaleX);
        Debug.Log(fovScaleY);
        fovChanger.rectTransform.sizeDelta = new Vector2(fovScaleX, fovScaleY);
        fovChanger.color = new Color(fovChanger.color.r, fovChanger.color.g, fovChanger.color.b, 200f / (interactableSolvedGoal - interactableSolvedCount));


        //if (interactableSolvedCount > 0)
        //{
        //
        //    fovChanger.rectTransform.sizeDelta = new Vector2(fovScaleX, fovScaleY);
        //}
        //else
        //{
        //    Debug.Log("FOVTEST");
        //    fovChanger.rectTransform.sizeDelta = new Vector2(330f, 154.5f);
        //}
    }
    public void SetActive()
    {
        mouseActive = true;
        activeTimer = 0f;
    }
}
