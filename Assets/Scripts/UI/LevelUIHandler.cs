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
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, UIAudio.Instance.bgmDefaultVolume);

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
