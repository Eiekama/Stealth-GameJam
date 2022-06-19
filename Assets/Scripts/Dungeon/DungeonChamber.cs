using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonChamber : MonoBehaviour
{
    public bool isLockedIn;
    public bool isFullyConnected;
    public DungeonChamber sourceChamber;

    public Connector[] Connectors { get; private set; }

    void Awake()
    {
        Connectors = GetComponentsInChildren<Connector>();
    }

    public IEnumerator SpawnRoomsProcess()
    {
        if (sourceChamber != null)
        {
            while (!sourceChamber.isFullyConnected) { yield return null; }

            if (Connectors.Length == 1)
            {
                isFullyConnected = true;
                yield break;
            }
        }

        for (int i = 0; i < Connectors.Length; i++)
        {
            yield return StartCoroutine(Connectors[i].SpawnAdjacentRoomProcess());
        }
        isFullyConnected = true;
    }
}
