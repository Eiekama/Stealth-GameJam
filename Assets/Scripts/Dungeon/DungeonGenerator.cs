using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance;

    [SerializeField] GameObject rootChamber;
    [SerializeField] GameObject endChamber;
    [SerializeField] int maxGenerationTime;
    public bool isFullyGenerated = false;

    [Header("Dungeon Prefabs")]
    public GameObject[] Hallway;
    public GameObject[] Corner;
    public GameObject[] Room;
    public GameObject[] SmallChamber;
    public GameObject[] MediumChamber;

    NavMeshSurface surface;

    DungeonChamber[] chambers = new DungeonChamber[1];
    List<DungeonChamber> incompleteChambers;
    int time;
    
    bool endChamberSpawned;

    void Awake()
    {
        Instance = this;

        time = maxGenerationTime;
        surface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        GameObject startChamber = Instantiate(rootChamber, transform);
        chambers[0] = startChamber.GetComponent<DungeonChamber>();
        chambers[0].isLockedIn = true;

        StartCoroutine(SpawnDungeonProcess());
        StartCoroutine(DoThingsAfterFullyGenerated());
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


        incompleteChambers = GetIncompleteChambers();
        while (!endChamberSpawned)
        {
            DungeonChamber chosenChamber = ChooseRandomChamber(incompleteChambers);
            yield return StartCoroutine(SpawnEndRoom(chosenChamber));
        }

        foreach (var chamber in incompleteChambers)
        {
            foreach (var connector in chamber.Connectors)
            {
                if (!connector.isConnected) { connector.SpawnWall(); }
                yield return null;
            }
            yield return null;
        }

        isFullyGenerated = true;
    }

    List<DungeonChamber> GetIncompleteChambers()
    {
        List<DungeonChamber> incompleteChambers = new List<DungeonChamber>();
        foreach (var chamber in chambers)
        {
            if (!chamber.isFullyConnected)
            {
                incompleteChambers.Add(chamber);
            }
        }
        return incompleteChambers;
    }

    DungeonChamber ChooseRandomChamber(List<DungeonChamber> components)
    {
        return components[Random.Range(0, components.Count)];
    }

    IEnumerator SpawnEndRoom(DungeonChamber chamber)
    {
        List<Connector> openConnectors = new List<Connector>();
        foreach (var connector in chamber.Connectors)
        {
            if (!connector.isConnected) { openConnectors.Add(connector); }
        }

        while (!endChamberSpawned)
        {
            if (openConnectors.Count == 0) { yield break; }

            Connector chosenConnecter = openConnectors[Random.Range(0, openConnectors.Count)];

            chosenConnecter.SpawnAdjacentRoom(endChamber, endChamber.GetComponentInChildren<Connector>().gameObject.transform);
            endChamber.GetComponentInChildren<Validator>().sourceConnector = chosenConnecter;

            yield return new WaitForSeconds(0.2f);

            if (chosenConnecter.isAttemptValid)
            {
                endChamberSpawned = true;
                yield break;
            }
            else
            {
                openConnectors.Remove(chosenConnecter);
            }

            yield return null;
        }
    }

    IEnumerator DoThingsAfterFullyGenerated()
    {
        while (!isFullyGenerated) { yield return null; }

        surface.BuildNavMesh();

        foreach (var enemy in GetComponentsInChildren<EnemyAI>(true))
        {
            enemy.gameObject.SetActive(true);
        }

        GetComponentInChildren<PlayerController>(true).gameObject.SetActive(true);
    }
}


/*
 * float prob = 0;

        foreach (var chamber in incompleteChambers)
        {
            prob += 100f / chambers.Length;

            for (int j = 0; j < chamber.Connectors.Length; j++)
            {
                Connector connector = chamber.Connectors[j];
                if (!connector.isConnected)
                {
                    if (!endChamberSpawned && Random.Range(0f, 100f) <= prob)
                    {
                        connector.SpawnAdjacentRoom(endChamber, endChamber.GetComponentInChildren<Connector>().gameObject.transform);
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
*/