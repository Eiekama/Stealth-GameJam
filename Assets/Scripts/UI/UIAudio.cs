using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    [SerializeField] AudioClip startButtonClip;
    [SerializeField] AudioClip otherButtonClip;
    [SerializeField] AudioClip pauseClip;
    [SerializeField] AudioClip gameoverClip;
    [SerializeField] AudioClip pickupClip;
    [SerializeField] AudioClip winClip;

    public AudioSource bgmSource;
    [SerializeField] AudioSource uiSource;

    [Range(0f, 1f)]
    public float bgmDefaultVolume = .1f;

    public GameObject player;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        bgmSource.volume = 0;
    }

    private void Start()
    {
        GetButtons();
        SceneManager.activeSceneChanged += GetButtons;

        StartFade(bgmSource, 5f, bgmDefaultVolume);
    }

    private void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }

    public void PlayStartButtonAudio()
    {
        uiSource.PlayOneShot(startButtonClip);
    }

    public void PlayOtherButtonAudio()
    {
        uiSource.PlayOneShot(otherButtonClip, .5f);
    }

    public void PlayPauseAudio()
    {
        uiSource.PlayOneShot(pauseClip);
    }

    public void PlayGameoverAudio()
    {
        uiSource.PlayOneShot(gameoverClip);
    }

    public void PlayPickupAudio()
    {
        uiSource.PlayOneShot(pickupClip);
    }

    public void PlayWinAudio()
    {
        uiSource.PlayOneShot(winClip);
    }

    public void StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        StartCoroutine(Fade(audioSource, duration, targetVolume));
    }

    IEnumerator Fade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    void GetButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (var button in buttons)
        {
            if (button.gameObject.name == "Start Button") { button.onClick.AddListener(PlayStartButtonAudio); }
            else { button.onClick.AddListener(PlayOtherButtonAudio); }
        }
    }

    void GetButtons(Scene current, Scene next)
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (var button in buttons)
        {
            if (button.gameObject.name == "Start Button") { button.onClick.AddListener(PlayStartButtonAudio); }
            else { button.onClick.AddListener(PlayOtherButtonAudio); }
        }
    }
}
