using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUIHandler : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance.isPaused = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(1);
    }
}
