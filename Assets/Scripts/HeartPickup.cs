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
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            player.hp++;
            player.OnHealthIncrease.Invoke();
            foreach (var particle in burst)
            {
                particle.Play();
            }
            gameObject.SetActive(false);
        }
    }
}
