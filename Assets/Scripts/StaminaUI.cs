using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    PlayerController player;
    Image staminaBar;
    Canvas canvas;

    [Header("Colours")]
    [ColorUsage(true, true)]
    [SerializeField] Color fillColor;
    [ColorUsage(true, true)]
    [SerializeField] Color recoveringColor;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        staminaBar = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        canvas.gameObject.transform.LookAt(canvas.gameObject.transform.position + Camera.main.gameObject.transform.forward, Camera.main.gameObject.transform.up);

        if (player.recovering && staminaBar.color != recoveringColor) { staminaBar.color = recoveringColor; }
        else if (!player.recovering && staminaBar.color != fillColor) { staminaBar.color = fillColor; }

        if (canvas.gameObject.GetComponent<CanvasGroup>().alpha != 0) { StaminaFiller(); }
    }

    void StaminaFiller()
    {
        staminaBar.fillAmount = player.Stamina / player.MaxStamina;
    }
}
