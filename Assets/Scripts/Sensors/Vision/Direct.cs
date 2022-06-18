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

        enemy.currentState = enemy.TryGetNewState(EnemyAI.State.Chase);
    }

    protected override void OnLosePlayer(Transform player)
    {
        base.OnLosePlayer(player);

        enemy.currentState = enemy.TryGetNewState(EnemyAI.State.Search);
    }
}
