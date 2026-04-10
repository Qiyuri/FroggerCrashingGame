using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] roadPrefabs;          // Voor de hoofdweg (center lane)
    public GameObject[] sideLanePrefabs;      // Voor de zijlanes (links en rechts)
    public Transform player;

    [Header("Chunk grootte")]
    [Tooltip("Hoe ver elke chunk zich uitstrekt in de rijrichting (Z-as)")]
    public float chunkLength = 20f;

    [Tooltip("Breedte van een chunk — afstand tussen lanes op de X-as")]
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
        // Spawn hoofdweg chunk in het midden
        GameObject centerPrefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)];
        Vector3 centerPos = new Vector3(0f, 0f, spawnZ);
        GameObject centerChunk = Instantiate(centerPrefab, centerPos, Quaternion.identity);
        activeChunks.Add(centerChunk);

        // Spawn zijlanes links en rechts als sideLanePrefabs zijn ingesteld
        if (sideLanePrefabs != null && sideLanePrefabs.Length > 0)
        {
            GameObject sidePrefabLeft = sideLanePrefabs[Random.Range(0, sideLanePrefabs.Length)];
            Vector3 leftPos = new Vector3(-chunkWidth, 0f, spawnZ);
            GameObject leftChunk = Instantiate(sidePrefabLeft, leftPos, Quaternion.identity);
            activeChunks.Add(leftChunk);

            GameObject sidePrefabRight = sideLanePrefabs[Random.Range(0, sideLanePrefabs.Length)];
            Vector3 rightPos = new Vector3(chunkWidth, 0f, spawnZ);
            GameObject rightChunk = Instantiate(sidePrefabRight, rightPos, Quaternion.identity);
            activeChunks.Add(rightChunk);
        }

        spawnZ += chunkLength;   // Volgende chunk begint hier
    }
}
