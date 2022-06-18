using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Components")]
    [SerializeField] Animator enemyAnim;
    [SerializeField] Animator enemyUIAnim;

    NavMeshAgent agent;
    Canvas enemyUICanvas;
    EnemyAudio enemyAudio;


    [Header("Enemy Stats")]
    public float viewconeRange;
    public float viewconeAngle;
    public float peripheralRange;
    public float peripheralAngle;
    public float sixthSenseRange;
    public float sixthSenseAngle;
    public float hearingThreshold;
    [SerializeField] float DefaultStoppingDistance = 0.5f;
    [SerializeField] float DefaultSpeed = .5f;

    [Header("Enemy State")]
    public State currentState;

    public enum State
    {
        Idle,
        Suspicious,
        Turn,
        Chase,
        Search,
        CheckSound
    }
    bool[] CalledOnceForState = new bool[Enum.GetNames(typeof(State)).Length];
    public UnityEvent OnChangeState = new UnityEvent();


    [Header("Patrol Path Waypoint")]
    [SerializeField] Waypoint currentWaypoint;

    bool changedDir;

    [Header("References to Player")]
    public Vector3 playerPos;
    public Vector3 lastSeenPos;


    Quaternion targetRot;

    Vector3 soundPos;

    bool isReacting;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = DefaultStoppingDistance;
        agent.speed = DefaultSpeed;

        enemyUICanvas = GetComponentInChildren<Canvas>();
        enemyAudio = GetComponentInChildren<EnemyAudio>();
    }

    void Start()
    {
        OnChangeState.AddListener(ResetVariables);

        enemyUICanvas.worldCamera = Camera.main;
    }

    void Update()
    {
        FaceCamera(enemyUICanvas);

        if (!isReacting)
        {
            switch (currentState)
            {
                case State.Idle:
                    Idle();
                    break;

                case State.Suspicious:
                    Suspicious();
                    break;

                case State.Turn:
                    Turn();
                    break;

                case State.Chase:
                    Chase();
                    break;

                case State.Search:
                    Search();
                    break;

                case State.CheckSound:
                    CheckSound();
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Noise")) //audio sensor
        {
            if (currentState != State.Chase && currentState != State.CheckSound)
            {
                float intensity = CalculateSoundIntensity(other.gameObject);

                if (intensity > hearingThreshold)
                {
                    currentState = TryGetNewState(State.CheckSound);
                }
            }
        }
    }

    float CalculateSoundIntensity(GameObject source)
    {
        soundPos = new Vector3(source.transform.position.x, 0, source.transform.position.z);

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(soundPos, path);
        float distance = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        if (source.GetComponent<NoiseSource>() == null) { Debug.LogError(source.name + " does not have NoiseSource script."); }

        return source.GetComponent<NoiseSource>().sourceIntensity / (distance * distance);
    }

    void FaceCamera(Canvas canvas)
    {
        if (!AllChildrenAreInactive(canvas.gameObject))
        {
            canvas.gameObject.transform.LookAt(canvas.gameObject.transform.position + canvas.worldCamera.gameObject.transform.forward, canvas.worldCamera.gameObject.transform.up);
        }
    }

    bool AllChildrenAreInactive(GameObject gameObject)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.activeInHierarchy) { return false; }
        }
        return true;
    }

    public State TryGetNewState(State newState)
    {
        if (newState == State.Idle)
        {
            if (currentState != State.Chase && currentState != State.Idle)
            {
                Debug.Log("State changed to Idle");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        else if (newState == State.Suspicious)
        {
            if (currentState == State.Idle || currentState == State.Turn || currentState == State.CheckSound)
            {
                Debug.Log("State changed to Suspicious");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        else if (newState == State.Turn)
        {
            if (currentState == State.Idle || currentState == State.Suspicious || currentState == State.CheckSound)
            {
                Debug.Log("State changed to Turn");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        else if (newState == State.Chase)
        {
            if (currentState != State.Chase)
            {
                Debug.Log("State changed to Chase");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        else if (newState == State.Search)
        {
            if (currentState == State.Chase)
            {
                Debug.Log("State changed to Search");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        else if (newState == State.CheckSound)
        {
            if (currentState == State.Idle || currentState == State.Turn || currentState == State.Search)
            {
                Debug.Log("State changed to CheckSound");
                OnChangeState.Invoke();
                return newState;
            }
            else { return currentState; }
        }

        Debug.LogError("Given state not equals to any considered in function. Check if forgot to add new else if statement.");
        return currentState;
    }

    void Idle()
    {
        if (!CalledOnceForState[(int)State.Idle])
        {
            enemyUIAnim.SetBool("Qn_b", false);

            OnChangeState.AddListener(React);

            enemyAnim.SetFloat("Speed_f", 0);

            CalledOnceForState[(int)State.Idle] = true;
        }

        if (currentWaypoint == null) { return; }

        agent.SetDestination(currentWaypoint.GetPosition());

        if (ReachedDestination())
        {
            if (!changedDir && currentWaypoint.nextWaypoint != null)
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
            }
            else if (changedDir && currentWaypoint.previousWaypoint != null)
            {
                currentWaypoint = currentWaypoint.previousWaypoint;
            }
            else
            {
                changedDir = !changedDir;
            }
        }
    }

    void Suspicious()
    {
        if (!CalledOnceForState[(int)State.Suspicious])
        {
            OnChangeState.RemoveListener(React);

            CalledOnceForState[(int)State.Suspicious] = true;
        }
        agent.SetDestination(playerPos);
    }

    void Turn()
    {
        if (!CalledOnceForState[(int)State.Turn])
        {
            OnChangeState.RemoveListener(React);

            Vector3 up = Vector3.Cross(transform.forward, playerPos - transform.position);
            targetRot = up.y < 0 ? transform.rotation * Quaternion.Euler(0, 180, 0) : transform.rotation * Quaternion.Euler(0, -180, 0);

            CalledOnceForState[(int)State.Turn] = true;
        }

        agent.ResetPath();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, agent.angularSpeed * Time.deltaTime);

        if (transform.rotation == targetRot && currentState == State.Turn)
        {
            currentState = TryGetNewState(State.Idle);
        }
    }

    void Chase()
    {
        if (!CalledOnceForState[(int)State.Chase])
        {
            enemyUIAnim.SetBool("Qn_b", false);
            enemyUIAnim.SetTrigger("Ex_t");
            enemyAudio.RoarAudio();

            OnChangeState.AddListener(React);

            agent.speed *= 2;
            agent.stoppingDistance = 1;
            enemyAnim.SetFloat("Speed_f", 2);

            CalledOnceForState[(int)State.Chase] = true;
        }

        agent.SetDestination(playerPos);

        if (agent.remainingDistance < agent.stoppingDistance && !GameManager.Instance.isGameOver)
        {
            GameManager.Instance.isGameOver = true;
            agent.isStopped = true;
            enemyAnim.SetTrigger("Attack_t");
        }
    }

    void Search()
    {
        if (!CalledOnceForState[(int)State.Search])
        {
            OnChangeState.RemoveListener(React);

            agent.isStopped = false;

            agent.stoppingDistance = UnityEngine.Random.Range(0.5f, 1.5f);
            enemyAnim.SetFloat("Speed_f", 0);

            CalledOnceForState[(int)State.Search] = true;
        }

        agent.SetDestination(lastSeenPos);

        if (ReachedDestination())
        {
            currentState = TryGetNewState(State.Idle);
        }
    }

    void CheckSound()
    {
        if (!CalledOnceForState[(int)State.CheckSound])
        {
            OnChangeState.AddListener(React);

            enemyAnim.SetFloat("Speed_f", 0);

            CalledOnceForState[(int)State.Idle] = true;
        }

        agent.SetDestination(soundPos);

        if (ReachedDestination())
        {
            currentState = TryGetNewState(State.Idle);
        }
    }

    void React()
    {
        if (!isReacting && !GameManager.Instance.isGameOver)
        {
            enemyUIAnim.SetBool("Qn_b", true);
            enemyAudio.ReactAudio();

            isReacting = true;
            agent.isStopped = true;
            enemyAnim.SetTrigger("React_t");

            StartCoroutine(WaitForReactEnd());
        }
    }

    IEnumerator WaitForReactEnd()
    {
        while (!enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("React")) { yield return null; } //wait for a few frames for the react anim to start playing

        while (enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("React")) { yield return null; } //returns true while react anim is playing

        isReacting = false;
        agent.isStopped = false;
    }

    bool ReachedDestination()
    {
        if (agent.pathPending) { return false; }
        if (agent.remainingDistance > agent.stoppingDistance) { return false; }
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) { return true; }
        else { return false; }
    }

    void ResetVariables()
    {
        agent.speed = DefaultSpeed;
        agent.stoppingDistance = DefaultStoppingDistance;

        for (int i = 0; i < CalledOnceForState.Length; i++)
        {
            if (CalledOnceForState[i]) { CalledOnceForState[i] = false; }
        }
    }
}