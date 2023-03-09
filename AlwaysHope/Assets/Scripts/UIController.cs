using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    private bool paused = false;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private List<CanvasGroup> canvasGroups;
    [SerializeField] private float fadeRate = 1.0f;

    [SerializeField] private List<Interactables> objects;

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
        objects = new List<Interactables>();
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Interactable");
        foreach(GameObject obj in tempList)
        {
            objects.Add(obj.GetComponent<Interactables>());
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            paused = !paused;
            for(int i = 0; i < objects.Count; i++)
            {
                if(objects[i] != null)
                {
                    objects[i].paused = paused;
                }
            }
            Time.timeScale = paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            pauseMenu.SetActive(paused);
        }
        if(currentStep < 5)
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
}
