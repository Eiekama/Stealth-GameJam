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
            Debug.Log(GetComponentInParent<DungeonChamber>().name + ": Alert source connector then destroy self");
            sourceConnector.isAttemptValid = false;
            Destroy(chamber.gameObject);
        }
    }
}
