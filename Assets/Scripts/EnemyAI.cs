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
    [SerializeField] float ReactTime;
    float reactTime;
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

    Vector3 soundPos;
    [SerializeField] Waypoint currentWaypoint;
    bool changedDir;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = DefaultStoppingDistance;
        reactTime = ReactTime;
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
                break;

            case State.Search:
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
            if (currentState == State.Idle)
            {
                soundPos = new Vector3(other.gameObject.transform.position.x, 0, other.gameObject.transform.position.z);
                agent.stoppingDistance = Random.Range(0.5f, 1.5f);

                Debug.Log("State changed to CheckSound");
                currentState = State.CheckSound; //state auto terminates after enemy moves to target pos (to write later) or if forcibly overwritten
            }
        }
    }

    void Idle()
    {
        agent.SetDestination(currentWaypoint.GetPosition());

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!changedDir && currentWaypoint.nextWaypoint != null)
                {
                    currentWaypoint = currentWaypoint.nextWaypoint;
                } else if (changedDir && currentWaypoint.previousWaypoint != null)
                {
                    currentWaypoint = currentWaypoint.previousWaypoint;
                } else
                {
                    changedDir = !changedDir;
                }
            }
        }
    }

    void Suspicious()
    {

    }

    void Chase()
    {

    }

    void Search()
    {

    }

    void CheckSound()
    {
        if (reactTime > 0)
        {
            reactTime -= Time.deltaTime;
        } else
        {
            agent.SetDestination(soundPos);

            if (!agent.pathPending) //check if destination is reached
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    { //Done
                        Debug.Log("State changed to Idle");
                        currentState = State.Idle;
                        reactTime = ReactTime;
                        agent.stoppingDistance = DefaultStoppingDistance;
                    }
                }
            }
        }
    }
}
