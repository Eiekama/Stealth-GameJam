using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject winScreen;

    [SerializeField] DungeonGenerator dungeon;

    public bool hasBeatenGame;
    public bool isGameOver;
    public bool isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && !isPaused && dungeon.isFullyGenerated)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            isPaused = true;
        }
        else if (pauseScreen.activeInHierarchy && !isPaused)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
        else if (hasBeatenGame)
        {
            Time.timeScale = 0;
            winScreen.SetActive(true);
        }
    }

    IEnumerator WaitForDungeonGeneration()
    {
        loadingScreen.SetActive(true);
        while (!dungeon.isFullyGenerated) { yield return null; }
        loadingScreen.SetActive(false);
    }
}
