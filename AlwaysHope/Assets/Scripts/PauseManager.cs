using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    /// <summary>
    /// Returns the user to the start screen whenever called
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1.0f;
        try
        {
            GameObject.Find("GameManager").GetComponent<JSONHandler>().SaveToJSON();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Exits the game whenever called
    /// </summary>
    public void ExitGame()
    {
        try
        {
            GameObject.Find("GameManager").GetComponent<JSONHandler>().SaveToJSON();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        Application.Quit();
    }
}
