using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartContainer : MonoBehaviour
{
    public UnityEvent OnDisappear = new UnityEvent();
    Animator heartAnim;

    void Awake()
    {
        heartAnim = GetComponent<Animator>();
    }

    void Start()
    {
        OnDisappear.AddListener(Disappear);
    }

    void OnEnable()
    {
        heartAnim.SetBool("IsActive_b", true);
    }

    void Disappear()
    {
        StartCoroutine(DisappearSequence());
    }

    IEnumerator DisappearSequence()
    {
        heartAnim.SetBool("IsActive_b", false);
        while (!heartAnim.GetCurrentAnimatorStateInfo(0).IsName("Disappear")) { yield return null; }
        while (heartAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < .9f) { yield return null; }
        gameObject.SetActive(false);
    }
}
