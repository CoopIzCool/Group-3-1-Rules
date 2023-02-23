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
        Destroy(GameObject.Find("Checkpoint Tracker")); // destroy checkpoint tracker so it can be recreated next time the game is started
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Exits the game whenever called
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
