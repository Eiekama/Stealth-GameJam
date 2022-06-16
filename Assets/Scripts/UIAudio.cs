using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    [SerializeField] AudioClip bgmClip;

    [SerializeField] AudioClip startButtonClip;
    [SerializeField] AudioClip otherButtonClip;
    [SerializeField] AudioClip pauseClip;

    AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GetButtons();
        SceneManager.activeSceneChanged += GetButtons;
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
        audioSource.PlayOneShot(startButtonClip);
    }

    public void PlayOtherButtonAudio()
    {
        audioSource.PlayOneShot(otherButtonClip, .5f);
    }

    public void PlayPauseAudio()
    {
        audioSource.PlayOneShot(pauseClip);
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
