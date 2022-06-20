using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] ParticleSystem[] burst;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<PlayerController>().hp++;
            foreach (var particle in burst)
            {
                particle.Play();
            }
            gameObject.SetActive(false);
        }
    }
}
