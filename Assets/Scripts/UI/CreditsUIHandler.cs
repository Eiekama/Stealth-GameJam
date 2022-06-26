using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsUIHandler : MonoBehaviour
{
    public void GoToMenu()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, UIAudio.Instance.bgmDefaultVolume);
        SceneManager.LoadScene(0);
    }
}
