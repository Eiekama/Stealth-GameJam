using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioClip leftFootstepClip;
    [SerializeField] AudioClip rightFootstepClip;
    [SerializeField] AudioClip leftFootRunClip;
    [SerializeField] AudioClip rightFootRunClip;
    [SerializeField] AudioClip leftFootCrouchClip;
    [SerializeField] AudioClip rightFootCrouchClip;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void LeftFootstepAudio()
    {
        audioSource.PlayOneShot(leftFootstepClip, .2f);
    }

    public void RightFootstepAudio()
    {
        audioSource.PlayOneShot(rightFootstepClip, .2f);
    }

    public void LeftFootRunAudio()
    {
        audioSource.PlayOneShot(leftFootRunClip, .3f);
    }

    public void RightFootRunAudio()
    {
        audioSource.PlayOneShot(rightFootRunClip, .3f);
    }

    public void LeftFootCrouchAudio()
    {
        audioSource.PlayOneShot(leftFootCrouchClip, .2f);
    }

    public void RightFootCrouchAudio()
    {
        audioSource.PlayOneShot(rightFootCrouchClip, .2f);
    }
}
