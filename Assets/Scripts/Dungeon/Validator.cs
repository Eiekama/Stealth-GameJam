using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validator : MonoBehaviour
{
    public Connector sourceConnector;
    DungeonChamber chamber;

    void Awake()
    {
        chamber = GetComponentInParent<DungeonChamber>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called");
        if (sourceConnector == null) { return; }

        // tell connector this room is not valid
        if (!chamber.isLockedIn)
        {
            sourceConnector.isAttemptValid = false;
            Destroy(chamber.gameObject);
        }
    }
}
