using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float viewconeRange;
    public float viewconeAngle;
    public float hearingThreshold;

    public enum State
    {
        Idle,
        Suspicious,
        Chase,
        Search,
        CheckSound
    }
    public State currentState;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Noise")) //audio sensor
        { //after implementing navmesh add distance calculation for intensity
            if (currentState == State.Idle)
            {
                Debug.Log("State changed to CheckSound");
                currentState = State.CheckSound; //state auto terminates after enemy moves to target pos (to write later) or if forcibly overwritten
            }
        }
    }
}
