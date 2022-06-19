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

    [Header("Reference to Dungeon")]
    [SerializeField] DungeonGenerator dungeon;

    GameObject player;
    PlayerController playerController;
    Vector3 spawnPos;
    Quaternion spawnRot;

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
        if (Input.GetKeyDown(KeyCode.Space) && !isPaused && dungeon.isFullyGenerated)
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
            if (finishedDeathAnim()) { RespawnPlayer(); }
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

    bool finishedDeathAnim()
    {
        if (!playerController.playerAnim.GetCurrentAnimatorStateInfo(0).IsTag("Death")) { return false; }
        if (playerController.playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) { return false; }
        return true;
    }

    void RespawnPlayer()
    {
        playerController.playerModel.gameObject.SetActive(false);
        gameOver = false;
        player.transform.position = spawnPos;
        player.transform.rotation = spawnRot;
        playerController.isDead = false;
        playerController.playerModel.gameObject.SetActive(true);
    }

    IEnumerator WaitForDungeonGeneration()
    {
        loadingScreen.SetActive(true);
        while (!dungeon.isFullyGenerated) { yield return null; }
        loadingScreen.SetActive(false);

        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        spawnPos = player.transform.position;
        spawnRot = player.transform.rotation;
    }
}
