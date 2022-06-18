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

        enemy.currentState = enemy.TryGetNewState(EnemyAI.State.Turn);
    }
}
