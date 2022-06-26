using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUIHandler : MonoBehaviour
{
    public void StartGame()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, 3f, 0f);
        SceneManager.LoadScene(1);
    }

    public void GoToCredits()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, .5f * UIAudio.Instance.bgmDefaultVolume);
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}