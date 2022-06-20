using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Validator : MonoBehaviour
{
    public Connector sourceConnector;
    [SerializeField] DungeonChamber chamber;
    DungeonGenerator generator;

    private void Start()
    {
        generator = transform.root.gameObject.GetComponent<DungeonGenerator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Validator>() == null) { return; }
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
