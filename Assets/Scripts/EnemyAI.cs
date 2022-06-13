using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] Animator enemyAnim;

    public float viewconeRange;
    public float viewconeAngle;
    public float peripheralRange;
    public float peripheralAngle;
    public float sixthSenseRange;
    public float sixthSenseAngle;
    public float hearingThreshold;
    [SerializeField] float DefaultStoppingDistance = 0.5f;
    [SerializeField] float DefaultSpeed = .5f;

    public enum State
    {
        Idle,
        Suspicious,
        Turn,
        Chase,
        Search,
        CheckSound
    }
    public State currentState;

    public UnityEvent OnChangeState;

    Dictionary<string, bool> ChangedVariablesFor = new Dictionary<string, bool>
    {
        { Enum.GetName(typeof(State), 0), false },
        { Enum.GetName(typeof(State), 1), false },
        { Enum.GetName(typeof(State), 2), false },
        { Enum.GetName(typeof(State), 3), false },
        { Enum.GetName(typeof(State), 4), false },
        { Enum.GetName(typeof(State), 5), false }
    };

    [SerializeField] Waypoint currentWaypoint;
    bool changedDir;

    public Vector3 playerPos;

    Quaternion targetRot;

    public Vector3 lastSeenPos;

    Vector3 soundPos;

    bool isReacting;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = DefaultStoppingDistance;
        agent.speed = DefaultSpeed;
        if (OnChangeState == null) { OnChangeState = new UnityEvent(); }
    }

    void Start()
    {
        OnChangeState.AddListener(ResetVariables);
    }

    void Update()
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called. Collider tag: " + other.tag);
        if (other.CompareTag("Noise")) //audio sensor
        {
            if (currentState != State.Chase && currentState != State.CheckSound)
            {
                soundPos = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z);

                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(soundPos, path);
                float distance = 0;
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }
                //Debug.Log("distance: " + distance);

                float intensity = other.GetComponent<NoiseSource>().sourceIntensity / (distance * distance);
                //Debug.Log("intensity: " + intensity);

                if (intensity > hearingThreshold)
                {
                    Debug.Log("State changed to CheckSound");
                    OnChangeState.Invoke();
                    currentState = State.CheckSound;
                }
            }
        }
    }

    // temporary functions so that enemy wont push around player when they catch up
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            agent.isStopped = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            agent.isStopped = false;
        }
    }

    void Idle()
    {
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
        if (!ChangedVariablesFor["Suspicious"])
        {
            React();

            ChangedVariablesFor["Suspicious"] = true;
        }

        if (!isReacting) { agent.SetDestination(playerPos); }
    }

    void Turn()
    {
        if (!ChangedVariablesFor["Turn"])
        {
            React();

            Vector3 up = Vector3.Cross(transform.forward, playerPos - transform.position);
            targetRot = up.y < 0 ? transform.rotation * Quaternion.Euler(0, 180, 0) : transform.rotation * Quaternion.Euler(0, -180, 0);

            ChangedVariablesFor["Turn"] = true;
        }

        if (!isReacting)
        {
            agent.ResetPath();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, agent.angularSpeed * Time.deltaTime);

            if (transform.rotation == targetRot && currentState == State.Turn)
            {
                Debug.Log("State changed to Idle");
                OnChangeState.Invoke();
                currentState = State.Idle;
            }
        }
    }

    void Chase()
    {
        if (!ChangedVariablesFor["Chase"])
        {
            React();

            agent.speed *= 2;
            enemyAnim.SetFloat("Speed_f", 2);

            ChangedVariablesFor["Chase"] = true;
        }

        if (!isReacting) { agent.SetDestination(playerPos); }
    }

    void Search()
    {
        if (!ChangedVariablesFor["Search"])
        {
            React();

            agent.stoppingDistance = UnityEngine.Random.Range(0.5f, 1.5f);
            enemyAnim.SetFloat("Speed_f", 0);

            ChangedVariablesFor["Search"] = true;
        }

        if (!isReacting)
        {
            agent.SetDestination(lastSeenPos);

            if (ReachedDestination())
            {
                Debug.Log("State changed to Idle");
                OnChangeState.Invoke();
                currentState = State.Idle;
            }
        }
    }

    void CheckSound()
    {
        if (!ChangedVariablesFor["CheckSound"])
        {
            React();

            ChangedVariablesFor["CheckSound"] = true;
        }

        if (!isReacting)
        {
            agent.SetDestination(soundPos);

            if (ReachedDestination())
            {
                Debug.Log("State changed to Idle");
                OnChangeState.Invoke();
                currentState = State.Idle;
            }
        }
    }

    void React()
    {
        isReacting = true;
        agent.isStopped = true;
        enemyAnim.SetTrigger("React_t");
        StartCoroutine(WaitForReactEnd());
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

        foreach (var state in Enum.GetNames(typeof(State)))
        {
            if (ChangedVariablesFor[state]) { ChangedVariablesFor[state] = false; }
        }
    }

    IEnumerator WaitForReactEnd()
    {
        while (!enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("React")) { yield return null; } //wait for a few frames for the react anim to start playing

        while (enemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("React")) { yield return null; } //returns true while react anim is playing
        isReacting = false;
        agent.isStopped = false;
    }
}