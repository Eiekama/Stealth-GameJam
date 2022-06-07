using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;

    public float viewconeRange;
    public float viewconeAngle;
    public float hearingThreshold;
    [SerializeField] float DefaultStoppingDistance = 0.5f;

    public enum State
    {
        Idle,
        Suspicious,
        Chase,
        Search,
        CheckSound
    }
    public State currentState;

    [SerializeField] Waypoint currentWaypoint;
    bool changedDir;

    public Vector3 playerPos;

    public Vector3 lastSeenPos;

    Vector3 soundPos;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = DefaultStoppingDistance;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Suspicious:
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

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Noise")) //audio sensor
        { //after implementing navmesh add distance calculation for intensity
            if (currentState != State.Chase && currentState != State.CheckSound)
            {
                soundPos = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z);
                agent.stoppingDistance = Random.Range(0.5f, 1.5f);

                Debug.Log("State changed to CheckSound");
                currentState = State.CheckSound;
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

    }

    void Chase()
    {
        agent.SetDestination(playerPos);
    }

    void Search()
    {
        agent.SetDestination(lastSeenPos);

        if (ReachedDestination())
        {
            Debug.Log("State changed to Idle");
            currentState = State.Idle;
            agent.stoppingDistance = DefaultStoppingDistance;
        }
    }

    void CheckSound()
    {
        agent.SetDestination(soundPos);

        if (ReachedDestination())
        {
            Debug.Log("State changed to Idle");
            currentState = State.Idle;
            agent.stoppingDistance = DefaultStoppingDistance;
        }
    }

    bool ReachedDestination()
    {
        if (agent.pathPending) { return false; }
        if (agent.remainingDistance > agent.stoppingDistance) { return false; }
        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) { return true; }
        else { return false; }
    }
}
