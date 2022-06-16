using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [SerializeField] AudioClip leftFootstepClip;
    [SerializeField] AudioClip rightFootstepClip;
    [SerializeField] AudioClip leftFootRunClip;
    [SerializeField] AudioClip rightFootRunClip;
    [SerializeField] AudioClip reactClip;
    [SerializeField] AudioClip roarClip;
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip squelchClip;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void LeftFootstepAudio()
    {
        audioSource.PlayOneShot(leftFootstepClip, .7f);
    }

    public void RightFootstepAudio()
    {
        audioSource.PlayOneShot(rightFootstepClip, .7f);
    }

    public void LeftFootRunAudio()
    {
        audioSource.PlayOneShot(leftFootRunClip);
    }

    public void RightFootRunAudio()
    {
        audioSource.PlayOneShot(rightFootRunClip);
    }

    public void ReactAudio()
    {
        audioSource.PlayOneShot(reactClip);
    }

    public void RoarAudio()
    {
        audioSource.PlayOneShot(roarClip);
    }

    public void AttackAudio()
    {
        audioSource.PlayOneShot(attackClip, .6f);
    }

    public void SquelchAudio()
    {
        audioSource.PlayOneShot(squelchClip, .8f);
    }
}
