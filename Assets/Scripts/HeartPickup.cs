using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [SerializeField] ParticleSystem[] burst;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DataManager.Instance.playerHp++;
            other.gameObject.GetComponentInParent<PlayerController>().OnHealthIncrease.Invoke();
            foreach (var particle in burst)
            {
                particle.Play();
            }

            UIAudio.Instance.PlayPickupAudio();

            gameObject.SetActive(false);
        }
    }
}
