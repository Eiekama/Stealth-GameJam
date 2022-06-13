using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Viewcone : MonoBehaviour
{
    [SerializeField] protected EnemyAI enemy;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    protected float range;
    protected float angle;
    [SerializeField] int triangleCount;

    Mesh viewcone;

    protected virtual void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        //assign range and angle in override
    }

    void Start()
    {
        viewcone = CreateMesh.Cone(range, angle, triangleCount);

        meshFilter.mesh = viewcone;
        meshCollider.sharedMesh = viewcone;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) //vision sensor
        {
            if (CanSeePlayer(other.gameObject.transform))
            {
                OnSeePlayer(other.gameObject.transform);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnLosePlayer(other.gameObject.transform);
        }
    }

    protected virtual void OnSeePlayer(Transform player)
    {
        enemy.playerPos = player.position;
        //update state changes in override
    }

    protected virtual void OnLosePlayer(Transform player)
    {
        enemy.lastSeenPos = player.position;
        //update state changes in override
    }

    bool CanSeePlayer(Transform player)
    {
        Vector3 direction = player.position - transform.position;

        Debug.DrawRay(transform.position, direction, Color.magenta);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hitResult, range))
        {
            //Debug.Log(hitResult.collider);

            if (hitResult.collider.CompareTag("Player")) { return true; }
        }
        return false;
    }
}
