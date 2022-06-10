using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] GameObject rootChamber;
    [SerializeField] GameObject endChamber;
    [SerializeField] int maxGenerationTime;
    [SerializeField] GameObject[] dungeonPrefabs;

    NavMeshSurface surface;

    DungeonChamber[] chambers = new DungeonChamber[1];
    int time;
    bool isFullyGenerated = false;
    bool endChamberSpawned;

    void Awake()
    {
        time = maxGenerationTime;
        surface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        GameObject startChamber = Instantiate(rootChamber, transform);
        chambers[0] = startChamber.GetComponent<DungeonChamber>();
        chambers[0].isLockedIn = true;
        StartCoroutine(SpawnDungeonProcess());
        StartCoroutine(BakeMesh());
    }

    IEnumerator SpawnDungeonProcess()
    {
        while (time > 0)
        {
            for (int i = 0; i < chambers.Length; i++)
            {
                if (!chambers[i].isFullyConnected)
                {
                    yield return StartCoroutine(chambers[i].SpawnRoomsProcess());
                }
                
                yield return null;
            }

            chambers = chambers.Concat(GetComponentsInChildren<DungeonChamber>()).ToArray();

            time--;
            yield return null;
        }

        float prob = 0;

        for (int i = 0; i < chambers.Length; i++)
        {
            prob += 100 / chambers.Length;
            if (chambers[i].isFullyConnected) { continue; }

            for (int j = 0; j < chambers[i].connectors.Length; j++)
            {
                Connector connector = chambers[i].connectors[j];
                if (!connector.isConnected)
                {
                    if (!endChamberSpawned && Random.Range(0f,100f) <= prob)
                    {
                        connector.SpawnAdjacentRoom(endChamber, endChamber.GetComponentInChildren<Connector>().transform);
                        endChamber.GetComponentInChildren<Validator>().sourceConnector = connector;
                        yield return new WaitForSeconds(0.2f);
                        if (connector.isAttemptValid)
                        {
                            endChamberSpawned = true;
                            continue;
                        }
                    }

                    connector.SpawnWall();
                }
                yield return null;
            }

            yield return null;
        }


        DungeonChamber finalChamber = chambers[chambers.Length - 1];
        finalChamber.connectors[finalChamber.connectors.Length - 1].SpawnAdjacentRoom(endChamber, endChamber.GetComponentInChildren<Connector>().transform);

        isFullyGenerated = true;
    }

    IEnumerator BakeMesh()
    {
        while (!isFullyGenerated) { yield return null; }
        surface.BuildNavMesh();
    }
}
