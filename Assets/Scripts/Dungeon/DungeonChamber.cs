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
}
