using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validator : MonoBehaviour
{
    public Connector sourceConnector;
    DungeonChamber chamber;
    DungeonGenerator generator;

    void Awake()
    {
        chamber = GetComponentInParent<DungeonChamber>();
    }

    private void Start()
    {
        generator = transform.root.gameObject.GetComponent<DungeonGenerator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter called");
        if (generator != null && !generator.isFullyGenerated)
        {
            if (sourceConnector == null) { return; }

            // tell connector this room is not valid
            if (!chamber.isLockedIn)
            {
                sourceConnector.isAttemptValid = false;
                Destroy(chamber.gameObject);
            }
        }
    }
}
