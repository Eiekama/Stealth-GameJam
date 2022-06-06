using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewcone : MonoBehaviour
{
    [SerializeField] EnemyAI enemy;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField] int triangleCount;

    Mesh viewcone;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;


    void Awake()
    {
        viewcone = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        CalculateMeshData();
    }

    // Start is called before the first frame update
    void Start()
    {
        viewcone.vertices = vertices;
        viewcone.uv = uv;
        viewcone.triangles = triangles;

        meshFilter.mesh = viewcone;
        meshCollider.sharedMesh = viewcone;
    }


    void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 direction = new Vector3(other.gameObject.transform.position.x - transform.position.x, 0, other.gameObject.transform.position.z - transform.position.z);
            Debug.DrawRay(transform.position, direction, Color.magenta);
            RaycastHit hitResult;
            if (Physics.Raycast(transform.position, direction, out hitResult, enemy.viewconeRange))
            {
                if (hitResult.collider.CompareTag("Player")) //player in line of sight
                {
                    if (enemy.currentState != EnemyAI.State.Chase)
                    {
                        Debug.Log("State changed to Chase");
                        enemy.currentState = EnemyAI.State.Chase;
                    }
                } else //player in cover
                {
                    if (enemy.currentState == EnemyAI.State.Chase)
                    {
                        Debug.Log("State changed to Idle");
                        enemy.currentState = EnemyAI.State.Idle;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && enemy.currentState == EnemyAI.State.Chase)
        {
            Debug.Log("State changed to Idle");
            enemy.currentState = EnemyAI.State.Idle;
        }
    }

    void CalculateMeshData()
    {
        Vector3 origin = Vector3.zero;
        float angle = 0;
        float angleIncrease;

        vertices = new Vector3[2 * (triangleCount + 2)];
        uv = new Vector2[vertices.Length];
        triangles = new int[triangleCount * 12 + 12];

        angleIncrease = enemy.viewconeAngle / triangleCount;

        //top plane
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i < triangleCount + 1; i++)
        {
            Vector3 vertex = origin + GetVectorFromAngle(angle) * enemy.viewconeRange;
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }

        //bottom plane
        for (int i = 0; i < triangleCount + 2; i++)
        {
            vertices[vertexIndex] = vertices[i] - Vector3.up;

            if (i > 1)
            {
                triangles[triangleIndex] = triangleCount + 2;
                triangles[triangleIndex + 1] = vertexIndex;
                triangles[triangleIndex + 2] = vertexIndex - 1;

                triangleIndex += 3;
            }
            vertexIndex++;
        }

        //side planes
        for (int i = 0; i < vertices.Length / 2 - 1; i++)
        {
            triangles[triangleIndex] = i;
            triangles[triangleIndex + 1] = i + triangleCount + 2;
            triangles[triangleIndex + 2] = i + 1;

            triangles[triangleIndex + 3] = i + 1;
            triangles[triangleIndex + 4] = i + triangleCount + 2;
            triangles[triangleIndex + 5] = i + 1 + triangleCount + 2;

            triangleIndex += 6;
        }

        //final side plane
        triangles[triangleIndex] = vertices.Length / 2 - 1;
        triangles[triangleIndex + 1] = triangleCount + 2;
        triangles[triangleIndex + 2] = 0;

        triangles[triangleIndex + 3] = triangleCount + 2;
        triangles[triangleIndex + 4] = vertices.Length / 2 - 1;
        triangles[triangleIndex + 5] = vertices.Length / 2 - 1 + triangleCount + 2;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
}
