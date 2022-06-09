using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] GameObject rootChamber;
    [SerializeField] int maxGenerationTime;
    [SerializeField] GameObject[] dungeonPrefabs;

    DungeonChamber[] chambers = new DungeonChamber[1];
    int time;

    void Awake()
    {
        time = maxGenerationTime;
    }

    void Start()
    {
        GameObject startChamber = Instantiate(rootChamber, transform);
        chambers[0] = startChamber.GetComponent<DungeonChamber>();
        chambers[0].isLockedIn = true;
        StartCoroutine(SpawnDungeonProcess());
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
        
        for (int i = 0; i < chambers.Length; i++)
        {
            for (int j = 0; j < chambers[i].connectors.Length; j++)
            {
                if (!chambers[i].connectors[j].isConnected)
                {
                    chambers[i].connectors[j].SpawnWall();
                }
                yield return null;
            }

            yield return null;
        }
        
    }
}
