using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Instance")]
    public static GameManager Instance;

    [Header("UI Elements")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject winScreen;
    [SerializeField] ParticleSystem[] winVFXs;

    [Header("Game States")]
    public bool hasBeatenGame;
    public bool gameOver;
    public bool isPaused;


    private void Awake()
    {
        Instance = this;

        pauseScreen.SetActive(false);
        winScreen.SetActive(false);

        Time.timeScale = 1;
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
        else if (winScreen.activeInHierarchy == false && hasBeatenGame)
        {
            PlayWinSequence();
        }
        else if (gameOver)
        {
            // might add gameover screen here later
            Debug.Log("gameover");
        }
    }

    void PauseGame()
    {
        UIAudio.Instance.PlayPauseAudio();
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        isPaused = true;
    }

    void UnpauseGame() // theres a separate button that sets isPaused to false
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }

    void PlayWinSequence()
    {
        Time.timeScale = 0;
        foreach (var particle in winVFXs)
        {
            particle.Play();
        }
        winScreen.SetActive(true);
    }

    IEnumerator WaitForDungeonGeneration()
    {
        loadingScreen.SetActive(true);
        while (!DungeonGenerator.Instance.isFullyGenerated) { yield return null; }
        loadingScreen.SetActive(false);
    }
}
