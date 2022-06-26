using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [NonReorderable]
    [SerializeField] GameObject[] hearts;

    void Start()
    {
        player.OnHealthIncrease.AddListener(AddHearts);
        player.OnHealthDecrease.AddListener(DecreaseHearts);

        StartCoroutine(WaitForDungeonGeneration());
    }

    void AddHearts()
    {
        if (DataManager.Instance.playerHp > hearts.Length) { return; }

        for (int i = 0; i < DataManager.Instance.playerHp; i++)
        {
            if (!hearts[i].activeInHierarchy) { hearts[i].SetActive(true); }
        }
    }

    void DecreaseHearts()
    {
        if (DataManager.Instance.playerHp >= hearts.Length) { return; }

        for (int i = DataManager.Instance.playerHp; i < hearts.Length; i++)
        {
            if (hearts[i].activeInHierarchy)
            { hearts[i].GetComponent<HeartContainer>().OnDisappear.Invoke(); }
        }
    }

    IEnumerator WaitForDungeonGeneration()
    {
        while (!DungeonGenerator.Instance.isFullyGenerated) { yield return null; }
        for (int i = 0; i < DataManager.Instance.playerHp; i++)
        {
            if (!hearts[i].activeInHierarchy) { hearts[i].SetActive(true); }
        }
    }
}
