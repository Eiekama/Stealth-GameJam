using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;

    [Range(0f, 5f)]
    public float width = 1f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position - width / 2f * transform.right;
        Vector3 maxBound = transform.position + width / 2f * transform.right;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }
}
