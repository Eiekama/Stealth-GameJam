using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixthSense : Viewcone
{
    protected override void Awake()
    {
        base.Awake();

        range = enemy.sixthSenseRange;
        angle = enemy.sixthSenseAngle;
    }

    protected override void OnSeePlayer(Transform player)
    {
        base.OnSeePlayer(player);

        if (enemy.currentState == EnemyAI.State.Idle || enemy.currentState == EnemyAI.State.CheckSound)
        {
            Debug.Log("State changed to Turn");
            enemy.OnChangeState.Invoke();
            enemy.currentState = EnemyAI.State.Turn;
        }
    }
}
