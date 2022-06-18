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

        enemy.currentState = enemy.TryGetNewState(EnemyAI.State.Suspicious);
    }

    protected override void OnLosePlayer(Transform player)
    {
        base.OnLosePlayer(player);

        enemy.currentState = enemy.TryGetNewState(EnemyAI.State.Idle);
    }
}
