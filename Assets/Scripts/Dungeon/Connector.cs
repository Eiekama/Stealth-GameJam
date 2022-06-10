using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] GameObject wall;

    [SerializeField] List<GameObject> ValidPrefabs; //could be array of enums?
    List<GameObject> validPrefabs = new List<GameObject>();

    public bool isConnected;
    public bool isAttemptValid = true;

    void Awake()
    {
        validPrefabs = ValidPrefabs;

        isConnected = false;
    }

    public IEnumerator SpawnAdjacentRoomProcess()
    {
        GameObject chosenPrefab;
        GameObject clonedPrefab = null;

        while (!isConnected)
        {
            chosenPrefab = ChooseRandomPrefab(validPrefabs);

            //spawn
            if (chosenPrefab == null)
            {
                SpawnWall();
                isAttemptValid = true;
            } else
            {
                Connector[] validConnectors = chosenPrefab.GetComponentsInChildren<Connector>();
                int chosenIndex = Random.Range(0, validConnectors.Length);
                Connector chosenConnector = validConnectors[chosenIndex];

                clonedPrefab = SpawnAdjacentRoom(chosenPrefab, chosenConnector.transform);

                // script communication
                Validator[] chosenValidators = clonedPrefab.GetComponentsInChildren<Validator>();
                for (int i = 0; i < chosenValidators.Length; i++)
                {
                    chosenValidators[i].sourceConnector = this;
                }
                Connector[] clonedConnectors = clonedPrefab.GetComponentsInChildren<Connector>();
                clonedConnectors[chosenIndex].isConnected = true;
            }

            yield return new WaitForSeconds(.2f);

            //validation
            if (!isAttemptValid)
            {
                validPrefabs.Remove(chosenPrefab);
            } else
            {
                if (clonedPrefab != null)
                {
                    clonedPrefab.GetComponent<DungeonChamber>().isLockedIn = true;
                    clonedPrefab.GetComponent<DungeonChamber>().sourceChamber = GetComponentInParent<DungeonChamber>();
                }
                isConnected = true;
            }
            yield return null;
        }
    }

    GameObject ChooseRandomPrefab(List<GameObject> list)
    {
        if (list.Count == 0 || list == null) { return null; }
        return list[Random.Range(0, validPrefabs.Count)];
    }

    public GameObject SpawnWall()
    {
        GameObject spawnedWall = Instantiate(wall, transform.position, transform.rotation);
        spawnedWall.transform.SetParent(transform.root, true);

        return spawnedWall;
    }

    public GameObject SpawnAdjacentRoom(GameObject prefab, Transform connector)
    {
        //calculate required rotation and position
        float yRot = Quaternion.Angle(transform.rotation, connector.rotation);
        if (yRot == 180) { yRot = 0; }
        else if (yRot == 0) { yRot = 180; }
            //check if clockwise or counterclockwise
        Vector3 up = Vector3.Cross(transform.forward, connector.forward);
        yRot = up.y < 0 ? -yRot : +yRot;

        Quaternion spawnRot = Quaternion.Euler(prefab.transform.rotation.eulerAngles + new Vector3(0, yRot, 0));
        Vector3 spawnPos = transform.position + Quaternion.Euler(0, yRot, 0) * (prefab.transform.position - connector.position);

        // spawn prefab
        GameObject spawnedPrefab = Instantiate(prefab, spawnPos, spawnRot);
        spawnedPrefab.transform.SetParent(transform.root, true);

        return spawnedPrefab;
    }
}