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
    public readonly float MaxStamina = 10f;

    [SerializeField] GameObject walkingNoise;
    [SerializeField] GameObject runningNoise;

    float currentSpeed;
    public bool isDead;
    float stamina;
    public float Stamina
    {
        get { return stamina;}
        private set
        {
            if (stamina > MaxStamina) { stamina = MaxStamina; }
            else if (stamina < 0) { stamina = 0; }
            else { stamina = value; }
        }
    }
    public bool recovering;
    public bool staminaFull;
    [SerializeField] Animator staminaAnim;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();

        stamina = MaxStamina;
        currentSpeed = walkingSpeed;
        staminaFull = true;
        recovering = false;
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

        if (!recovering && stamina <= 0) { recovering = true; }

        if (stamina < MaxStamina)
        {
            if (currentSpeed == walkingSpeed) { stamina += Time.deltaTime * 1f; }
            else if (direction == Vector3.zero) { stamina += Time.deltaTime * 2f; }
        }
        else if (!staminaFull && stamina >= MaxStamina)
        {
            staminaAnim.SetTrigger("FadeOut_t");

            recovering = false;
            staminaFull = true;
        }

        if (direction != Vector3.zero)
        {
            if (direction.sqrMagnitude > 1) { direction = direction.normalized; }
            playerRb.MovePosition(playerTransform.position + currentSpeed * Time.deltaTime * direction);

            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            if (currentSpeed == runningSpeed)
            {
                if (staminaFull)
                {
                    staminaAnim.SetTrigger("FadeIn_t");
                    staminaFull = false;
                }
                stamina -= Time.deltaTime * 3f;
            }
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
                if (currentSpeed != runningSpeed && stamina > 0 && !recovering)
                {
                    currentSpeed = runningSpeed;
                    playerAnim.SetFloat("Speed_f", runningSpeed);
                    if (playerAnim.GetBool("IsCrouching_b"))
                    {
                        playerAnim.SetBool("IsCrouching_b", false);
                    }
                }
                else if (currentSpeed != walkingSpeed && recovering)
                {
                    currentSpeed = walkingSpeed;
                    playerAnim.SetFloat("Speed_f", walkingSpeed);
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
