using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    private bool paused = false;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private List<CanvasGroup> canvasGroups;
    [SerializeField] private float fadeRate = 1.0f;

    [SerializeField] private MouseRaycast raycastMgr;
    [SerializeField] private List<Image> checklist;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private Sprite crossSprite;
    private List<bool> solvedList = new List<bool>();


    private bool fadeIn = false;
    private bool fadeOut = false;
    [SerializeField] private Vector2 mousePos;
    [SerializeField] private int currentStep = 0;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i < canvasGroups.Count; i++)
        {
            canvasGroups[i].alpha = 0;
        }
        foreach(Image image in checklist)
        {
            solvedList.Add(true);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            paused = !paused;
            for(int i = 0; i < raycastMgr.RequiredInteractables.Length; i++)
            {
                if(raycastMgr.RequiredInteractables[i] != null)
                {
                    raycastMgr.RequiredInteractables[i].GetComponent<Interactables>().paused = paused;
                }
            }
            for(int j = 0; j < raycastMgr.OptionalInteractables.Length; j++)
            {
                if (raycastMgr.OptionalInteractables[j] != null)
                {
                    raycastMgr.OptionalInteractables[j].GetComponent<Interactables>().paused = paused;
                }
            }
            Time.timeScale = paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            pauseMenu.SetActive(paused);
        }
        // Check if objectives are solved
        for (int i = 0; i < raycastMgr.OptionalInteractables.Length; i++)
        {
            if (!raycastMgr.OptionalInteractables[i].GetComponent<Interactables>().isSolved)
            {
                solvedList[4] = false;
                break;
            }
        }
        bool photosSolved = true;
        bool laundrySolved = true;
        bool bookSolved = true;
        bool headphonesSolved = true;
        for (int j = 0; j < raycastMgr.RequiredInteractables.Length; j++)
        {
            //0 - 2 = photos
            if(j <= 2)
            {
                if(photosSolved)
                {
                    photosSolved = raycastMgr.RequiredInteractables[j].GetComponent<Interactables>().isSolved;
                    solvedList[0] = photosSolved;
                }
            }
            //3 - 6 = laundry
            if (j >= 3 && j <= 6)
            {
                if (laundrySolved)
                {
                    laundrySolved = raycastMgr.RequiredInteractables[j].GetComponent<Interactables>().isSolved;
                    solvedList[1] = laundrySolved;
                }
            }
            //7 - 10 = books
            if (j >= 7 && j <= 10)
            {
                if (bookSolved)
                {
                    bookSolved = raycastMgr.RequiredInteractables[j].GetComponent<Interactables>().isSolved;
                    solvedList[2] = bookSolved;
                }
            }
            //11 = headphones
            if (j == 11)
            {
                if (headphonesSolved)
                {
                    headphonesSolved = raycastMgr.RequiredInteractables[j].GetComponent<Interactables>().isSolved;
                    solvedList[3] = headphonesSolved;
                }
            }
        }
        for (int i = 0; i < checklist.Count; i++)
        {
            checklist[i].sprite = (solvedList[i] ? checkSprite : crossSprite);
        }
        if (currentStep < 6)
        {
            switch(currentStep)
            {
                case 0:
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) fadeOut = true;
                    break;
                case 1:
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) fadeOut = true;
                    break;
                case 2:
                    if (GetScrollWheelInput()) fadeOut = true;
                    break;
                case 3:
                    if (GetMouseClickMove()) fadeOut = true;
                    break;
                case 4:
                    if (Input.GetMouseButton(0) && GetScrollWheelInput()) fadeOut = true;
                    break;
                default:
                    break;
            }
            if (fadeOut)
            {
                canvasGroups[currentStep].alpha -= fadeRate * Time.deltaTime;
                fadeIn = false;
            }
            if (fadeIn)
            {
                canvasGroups[currentStep].alpha += fadeRate * Time.deltaTime;
            }
            if (currentStep+1 < canvasGroups.Count && canvasGroups[currentStep].alpha <= 0)
            {
                fadeOut = false;
                canvasGroups[currentStep].gameObject.SetActive(false);
                currentStep++;
                if(currentStep == 3 || currentStep == 4)
                {
                    mousePos = Input.mousePosition;
                }
                canvasGroups[currentStep].gameObject.SetActive(true);
                fadeIn = true;
            }
            if (canvasGroups[currentStep].alpha == 1)
            {
                fadeIn = false;
            }
        }
    }

    bool GetScrollWheelInput()
    {
        return Input.mouseScrollDelta != Vector2.zero;
    }

    bool GetMouseClickMove()
    {
        bool mouseMove = (!mousePos.Equals(Input.mousePosition));
        //Debug.Log("Mouse Move: " + mouseMove);
        mousePos = Input.mousePosition;
        return Input.GetMouseButton(0) && mouseMove;
    }

    void SetSprite()
    {

    }
}
