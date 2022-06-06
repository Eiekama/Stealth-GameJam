using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float viewconeRange;
    public float viewconeAngle;

    public enum State
    {
        Idle,
        Suspicious,
        Chase,
        Search
    }
    public State currentState;
}
