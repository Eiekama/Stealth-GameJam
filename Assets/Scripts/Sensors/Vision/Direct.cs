using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direct : Viewcone
{
    protected override void Awake()
    {
        base.Awake();

        range = enemy.viewconeRange;
        angle = enemy.viewconeAngle;
    }

    protected override void OnSeePlayer(Transform player)
    {
        base.OnSeePlayer(player);

        if (enemy.currentState != EnemyAI.State.Chase)
        {
            Debug.Log("State changed to Chase");
            enemy.OnChangeState.Invoke();
            enemy.currentState = EnemyAI.State.Chase;
        }
    }

    protected override void OnLosePlayer(Transform player)
    {
        base.OnLosePlayer(player);

        if (enemy.currentState == EnemyAI.State.Chase)
        {
            Debug.Log("State changed to Search");
            enemy.OnChangeState.Invoke();
            enemy.currentState = EnemyAI.State.Search;
        }
    }
}
