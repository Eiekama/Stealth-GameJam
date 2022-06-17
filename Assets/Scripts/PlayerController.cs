using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    public Transform playerModel;
    public Animator playerAnim;
    [SerializeField] Transform cameraRoot;
    [SerializeField] Animator staminaAnim;

    [Header("Player Movement Stats")]
    [SerializeField] float crouchingSpeed;
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float rotationSpeed;

    [Header("Stamina")]
    [SerializeField] float runningLoseRate;
    [SerializeField] float idleRecoveryRate;
    [SerializeField] float walkingRecoveryRate;

    public readonly float MaxStamina = 10f;
    float stamina;
    public float Stamina
    {
        get { return stamina; }
        private set
        {
            if (stamina > MaxStamina) { stamina = MaxStamina; }
            else if (stamina < 0) { stamina = 0; }
            else { stamina = value; }
        }
    }


    [Header("Noise Sources")]
    [SerializeField] GameObject walkingNoise;
    [SerializeField] GameObject runningNoise;

    [Header("Player States")]
    public bool isDead;
    public bool recovering;
    public bool staminaFull;

    public enum MovementState
    {
        Idle,
        Walking,
        Running,
        Crouch_Idle,
        Crouch_Walking
    }
    MovementState currentMovementState;
    MovementState previousMovementState;
    bool[] CalledOnceForMovementState = new bool[Enum.GetNames(typeof(MovementState)).Length];
    UnityEvent OnChangedMovementState = new UnityEvent();

    Rigidbody playerRb;

    float currentSpeed;

    bool isCrouching;

    float horizontalInput;
    float verticalInput;

   

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();

        stamina = MaxStamina;
        currentSpeed = walkingSpeed;
    }

    void Start()
    {
        VirtualCameraScript.virtualCam.Follow = cameraRoot;
        VirtualCameraScript.virtualCam.LookAt = cameraRoot;

        UIAudio.Instance.player = gameObject;

        OnChangedMovementState.AddListener(ResetCalledOnceBooleans);
    }

    void Update()
    {
        if (!GameManager.Instance.hasBeatenGame && !GameManager.Instance.isGameOver && !GameManager.Instance.isPaused)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            previousMovementState = currentMovementState;
            currentMovementState = GetCurrentMovementState();

            if (previousMovementState != currentMovementState) { OnChangedMovementState.Invoke(); }

            ManagePlayerMovement();

            ManageStaminaState();
        }

        ManageDeath();
    }

    void FixedUpdate()
    {
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        MoveTowards(direction);
    }

    void MoveTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) { return; }
        if (direction.sqrMagnitude > 1) { direction = direction.normalized; }
        playerRb.MovePosition(playerModel.position + currentSpeed * Time.deltaTime * direction);

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    MovementState GetCurrentMovementState()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching) { isCrouching = true; }
            else { isCrouching = false; }
        }

        if (Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput) == 0) { return isCrouching ? MovementState.Crouch_Idle : MovementState.Idle; }
        else // player is moving
        {
            if (recovering || !Input.GetKey(KeyCode.X)) { return isCrouching? MovementState.Crouch_Walking: MovementState.Walking; }
            else { return MovementState.Running; }
        }
    }

    void ManagePlayerMovement()
    {
        switch (currentMovementState)
        {
            case MovementState.Idle:
                if (!CalledOnceForMovementState[(int)MovementState.Idle])
                {
                    playerAnim.SetBool("IsCrouching_b", false);

                    currentSpeed = 0;
                    playerAnim.SetFloat("Speed_f", 0);

                    walkingNoise.SetActive(false);
                    runningNoise.SetActive(false);

                    CalledOnceForMovementState[(int)MovementState.Idle] = true;
                }

                if (stamina < MaxStamina) { stamina += Time.deltaTime * idleRecoveryRate; }

                break;

            case MovementState.Walking:
                if (!CalledOnceForMovementState[(int)MovementState.Walking])
                {
                    playerAnim.SetBool("IsCrouching_b", false);

                    currentSpeed = walkingSpeed;
                    playerAnim.SetFloat("Speed_f", walkingSpeed);

                    walkingNoise.SetActive(true);
                    runningNoise.SetActive(false);

                    CalledOnceForMovementState[(int)MovementState.Walking] = true;
                }

                if (stamina < MaxStamina) { stamina += Time.deltaTime * walkingRecoveryRate; }

                break;

            case MovementState.Running:
                if (!CalledOnceForMovementState[(int)MovementState.Running])
                {
                    playerAnim.SetBool("IsCrouching_b", false);

                    currentSpeed = runningSpeed;
                    playerAnim.SetFloat("Speed_f", runningSpeed);

                    walkingNoise.SetActive(false);
                    runningNoise.SetActive(true);

                    CalledOnceForMovementState[(int)MovementState.Running] = true;
                }

                stamina -= Time.deltaTime * runningLoseRate;

                break;

            case MovementState.Crouch_Idle:
                if (!CalledOnceForMovementState[(int)MovementState.Crouch_Idle])
                {
                    playerAnim.SetBool("IsCrouching_b", true);

                    currentSpeed = 0;
                    playerAnim.SetFloat("Speed_f", 0);

                    walkingNoise.SetActive(false);
                    runningNoise.SetActive(false);

                    CalledOnceForMovementState[(int)MovementState.Crouch_Idle] = true;
                }

                if (stamina < MaxStamina) { stamina += Time.deltaTime * idleRecoveryRate; }

                break;

            case MovementState.Crouch_Walking:
                if (!CalledOnceForMovementState[(int)MovementState.Crouch_Walking])
                {
                    playerAnim.SetBool("IsCrouching_b", true);

                    currentSpeed = walkingSpeed;
                    playerAnim.SetFloat("Speed_f", walkingSpeed);

                    walkingNoise.SetActive(false);
                    runningNoise.SetActive(false);

                    CalledOnceForMovementState[(int)MovementState.Crouch_Walking] = true;
                }

                if (stamina < MaxStamina) { stamina += Time.deltaTime * walkingRecoveryRate; }

                break;
        }
    }

    void ManageStaminaState()
    {
        if (!recovering && stamina <= 0) { recovering = true; }

        if (staminaFull && currentMovementState == MovementState.Running)
        {
            staminaAnim.SetTrigger("FadeIn_t");
            staminaFull = false;
        }
        else if (!staminaFull && stamina >= MaxStamina)
        {
            staminaAnim.SetTrigger("FadeOut_t");

            recovering = false;
            staminaFull = true;
        }
    }

    void ManageDeath()
    {
        if (GameManager.Instance.isGameOver && !isDead)
        {
            isDead = true;
            playerAnim.SetTrigger("DeathForward_t");
        }
    }

    void ResetCalledOnceBooleans()
    {
        for (int i = 0; i < CalledOnceForMovementState.Length; i++)
        {
            CalledOnceForMovementState[i] = false;
        }
    }
}