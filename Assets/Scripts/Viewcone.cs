using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewcone : MonoBehaviour
{
    [SerializeField] EnemyAI enemy;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField] int triangleCount;

    UnityEngine.Mesh viewcone;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        viewcone = MeshMethods.CreateCone(enemy.viewconeRange, enemy.viewconeAngle, triangleCount);

        meshFilter.mesh = viewcone;
        meshCollider.sharedMesh = viewcone;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) //vision sensor
        {
            Vector3 direction = new Vector3(other.gameObject.transform.position.x - transform.position.x, 0, other.gameObject.transform.position.z - transform.position.z);

            Debug.DrawRay(transform.position, direction, Color.magenta);

            if (Physics.Raycast(transform.position, direction, out RaycastHit hitResult, enemy.viewconeRange))
            {
                if (hitResult.collider.CompareTag("Player")) //player in line of sight
                {
                    enemy.playerPos = other.transform.position;

                    if (enemy.currentState != EnemyAI.State.Chase)
                    {
                        Debug.Log("State changed to Chase");
                        enemy.currentState = EnemyAI.State.Chase;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && enemy.currentState == EnemyAI.State.Chase)
        {
            enemy.lastSeenPos = other.transform.position;

            Debug.Log("State changed to Search");
            enemy.currentState = EnemyAI.State.Search;
        }
    }
}
