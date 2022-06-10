using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChamber : MonoBehaviour
{
    public bool isLockedIn;
    public bool isFullyConnected;
    public DungeonChamber sourceChamber;

    public Connector[] connectors { get; private set; }

    void Awake()
    {
        connectors = GetComponentsInChildren<Connector>();
    }

    public IEnumerator SpawnRoomsProcess()
    {
        if (sourceChamber != null)
        {
            while (!sourceChamber.isFullyConnected) { yield return null; }

            if (connectors.Length == 1)
            {
                isFullyConnected = true;
                yield break;
            }
        }

        for (int i = 0; i < connectors.Length; i++)
        {
            yield return StartCoroutine(connectors[i].SpawnAdjacentRoomProcess());
        }
        isFullyConnected = true;
    }
}
