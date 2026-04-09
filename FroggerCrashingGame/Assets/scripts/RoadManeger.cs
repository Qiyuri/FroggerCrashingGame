using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] roadPrefabs;
    public Transform player;

    [Header("Chunk grootte")]
    [Tooltip("Hoe ver elke chunk zich uitstrekt in de rijrichting (Z-as)")]
    public float chunkLength = 20f;

    [Tooltip("Breedte van een chunk — alleen nodig als je meerdere rijbanen naast elkaar wil")]
    public float chunkWidth = 10f;

    [Header("Spawn afstand")]
    [Tooltip("Hoeveel chunks vooruit worden gespawnd")]
    public int chunksAhead = 5;

    [Tooltip("Hoeveel chunks achter de speler bewaard blijven voor despawn")]
    public int chunksToKeepBehind = 2;

    private List<GameObject> activeChunks = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        spawnZ = 0f;
        for (int i = 0; i < chunksAhead + chunksToKeepBehind; i++)
            SpawnChunk();
    }

    void Update()
    {
        // Spawn bijhouden: zolang de verste chunk niet ver genoeg voor is
        while (spawnZ < player.position.z + chunksAhead * chunkLength)
            SpawnChunk();

        // Despawn chunks die te ver achter de speler liggen
        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            if (activeChunks[i] == null) { activeChunks.RemoveAt(i); continue; }

            float chunkBack = activeChunks[i].transform.position.z + chunkLength;
            if (chunkBack < player.position.z - chunksToKeepBehind * chunkLength)
            {
                Destroy(activeChunks[i]);
                activeChunks.RemoveAt(i);
            }
        }
    }

    void SpawnChunk()
    {
        GameObject prefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)];

        // Spawn op de huidige Z positie, gecentreerd op X=0 (of pas aan voor meerdere rijbanen)
        Vector3 pos = new Vector3(0f, 0f, spawnZ);
        GameObject chunk = Instantiate(prefab, pos, Quaternion.identity);
        activeChunks.Add(chunk);

        spawnZ += chunkLength;   // Volgende chunk begint precies hier
    }
}