using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peripheral : Viewcone
{
    protected override void Awake()
    {
        base.Awake();

        range = enemy.peripheralRange;
        angle = enemy.peripheralAngle;
    }

    protected override void OnSeePlayer(Transform player)
    {
        base.OnSeePlayer(player);

        if (enemy.currentState == EnemyAI.State.Idle || enemy.currentState == EnemyAI.State.CheckSound)
        {
            Debug.Log("State changed to Suspicious");
            enemy.OnChangeState.Invoke();
            enemy.currentState = EnemyAI.State.Suspicious;
        }
    }

    protected override void OnLosePlayer(Transform player)
    {
        base.OnLosePlayer(player);

        if (enemy.currentState == EnemyAI.State.Suspicious)
        {
            Debug.Log("State changed to Idle");
            enemy.OnChangeState.Invoke();
            enemy.currentState = EnemyAI.State.Idle;
        }
    }
}
