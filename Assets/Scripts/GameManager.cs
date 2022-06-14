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

    [SerializeField] GameObject playerPrefab;
    GameObject player;
    Vector3 spawnPos;
    Quaternion spawnRot;

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
        else if (isGameOver)
        {
            if (!player.GetComponent<PlayerController>().playerAnim.GetCurrentAnimatorStateInfo(0).IsTag("Death")) { return; }
            if (player.GetComponent<PlayerController>().playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) { return; }
            Destroy(player.gameObject);
            isGameOver = false;
            player = Instantiate(playerPrefab, spawnPos, spawnRot);
        }
    }

    IEnumerator WaitForDungeonGeneration()
    {
        loadingScreen.SetActive(true);
        while (!dungeon.isFullyGenerated) { yield return null; }
        loadingScreen.SetActive(false);

        player = GameObject.Find("Player");
        Transform playerTransform = player.gameObject.transform;
        spawnPos = playerTransform.position;
        spawnRot = playerTransform.rotation;
    }
}
