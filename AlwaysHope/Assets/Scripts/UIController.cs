using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    private bool paused = false;
    [SerializeField] private GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f; // If paused is true, stop time scale, if it is false, set the timescale to normal values
            pauseMenu.SetActive(paused);
        }
    }
}
