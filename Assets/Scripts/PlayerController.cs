using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator playerAnim;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform camRoot;
    Rigidbody playerRb;

    float horizontalInput;
    float verticalInput;

    [SerializeField] float crouchingSpeed;
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float rotationSpeed;

    [SerializeField] GameObject walkingNoise;
    [SerializeField] GameObject runningNoise;

    float currentSpeed;
    public bool isDead;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        currentSpeed = walkingSpeed;
    }

    private void Start()
    {
        VirtualCameraScript.virtualCam.Follow = camRoot;
        VirtualCameraScript.virtualCam.LookAt = camRoot;

        UIAudio.Instance.player = gameObject;
    }

    void Update()
    {
        if (!GameManager.Instance.hasBeatenGame && !GameManager.Instance.isGameOver && !GameManager.Instance.isPaused)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (currentSpeed == walkingSpeed)
            {
                if (!walkingNoise.activeInHierarchy) { walkingNoise.SetActive(true); }
                if (runningNoise.activeInHierarchy) { runningNoise.SetActive(false); }
            }
            else if (currentSpeed == runningSpeed)
            {
                if (walkingNoise.activeInHierarchy) { walkingNoise.SetActive(false); }
                if (!runningNoise.activeInHierarchy) { runningNoise.SetActive(true); }
            }
            else
            {
                if (walkingNoise.activeInHierarchy) { walkingNoise.SetActive(false); }
                if (runningNoise.activeInHierarchy) { runningNoise.SetActive(false); }
            }
        }

        ManageAnimations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

        if (direction != Vector3.zero)
        {
            if (direction.sqrMagnitude > 1) { direction = direction.normalized; }
            playerRb.MovePosition(playerTransform.position + currentSpeed * Time.deltaTime * direction);

            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

    }

    void ManageAnimations()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!playerAnim.GetBool("IsCrouching_b"))
            {
                playerAnim.SetBool("IsCrouching_b", true);
            }
            else
            {
                playerAnim.SetBool("IsCrouching_b", false);
            }
        }

        if (Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput) > 0) //if player is moving
        {
            if (Input.GetKey(KeyCode.X))
            {
                if (currentSpeed != runningSpeed)
                {
                    currentSpeed = runningSpeed;
                    playerAnim.SetFloat("Speed_f", runningSpeed);
                    if (playerAnim.GetBool("IsCrouching_b"))
                    {
                        playerAnim.SetBool("IsCrouching_b", false);
                    }
                }
            }
            else if (playerAnim.GetBool("IsCrouching_b"))
            {
                if (currentSpeed != crouchingSpeed)
                {
                    currentSpeed = crouchingSpeed;
                    playerAnim.SetFloat("Speed_f", crouchingSpeed);
                }
            }
            else
            {
                if (currentSpeed != walkingSpeed)
                {
                    currentSpeed = walkingSpeed;
                    playerAnim.SetFloat("Speed_f", walkingSpeed);
                }
            }
        }
        else // player not moving
        {
            if (currentSpeed != 0)
            {
                currentSpeed = 0;
                playerAnim.SetFloat("Speed_f", 0);
            }
        }

        if (GameManager.Instance.isGameOver && !isDead)
        {
            isDead = true;
            playerAnim.SetTrigger("DeathForward_t");
        }
    }
}
