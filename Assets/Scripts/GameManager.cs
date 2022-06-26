using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Instance")]
    public static GameManager Instance;

    [Header("UI Elements")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject levelTextCanvas;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameoverScreen;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject winScreen;
    [SerializeField] ParticleSystem[] winVFXs;

    [Header("Game States")]
    public bool hasBeatenGame;
    public bool gameOver;
    public bool isPaused;
    public UnityEvent OnWin = new UnityEvent();
    public UnityEvent OnGameover = new UnityEvent();


    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1;

        OnWin.AddListener(PlayWinSequence);
        OnGameover.AddListener(Gameover);
    }

    private void Start()
    {
        StartCoroutine(WaitForDungeonGeneration());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPaused && DungeonGenerator.Instance.isFullyGenerated)
        {
            PauseGame();
        }
        else if (pauseScreen.activeInHierarchy && !isPaused)
        {
            UnpauseGame();
        }
    }

    void PauseGame()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, .5f * UIAudio.Instance.bgmDefaultVolume);

        UIAudio.Instance.PlayPauseAudio();
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        isPaused = true;
    }

    void UnpauseGame() // theres a separate button that sets isPaused to false
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, UIAudio.Instance.bgmDefaultVolume);

        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    void PlayWinSequence()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, .5f * UIAudio.Instance.bgmDefaultVolume);

        UIAudio.Instance.PlayWinAudio();
        Time.timeScale = 0;
        foreach (var particle in winVFXs)
        {
            particle.Play();
        }
        winScreen.SetActive(true);

        DataManager.Instance.maxGenerationTime++;
    }

    void Gameover()
    {
        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, .5f, .5f * UIAudio.Instance.bgmDefaultVolume);

        UIAudio.Instance.PlayGameoverAudio();
        Time.timeScale = 0;
        scoreText.text = "Highest Level: " + (DataManager.Instance.maxGenerationTime - DataManager.startMaxGenerationTime);
        gameoverScreen.SetActive(true);

        DataManager.Instance.maxGenerationTime = DataManager.startMaxGenerationTime;
        DataManager.Instance.playerHp = 1;
    }

    IEnumerator WaitForDungeonGeneration()
    {
        loadingScreen.SetActive(true);
        while (!DungeonGenerator.Instance.isFullyGenerated) { yield return null; }
        loadingScreen.SetActive(false);

        levelTextCanvas.GetComponentInChildren<Text>(true).text = "Level " + (DataManager.Instance.maxGenerationTime - DataManager.startMaxGenerationTime + 1);
        levelTextCanvas.GetComponent<Animator>().SetTrigger("ShowText_t");

        UIAudio.Instance.StartFade(UIAudio.Instance.bgmSource, 10f, UIAudio.Instance.bgmDefaultVolume);
    }
}
