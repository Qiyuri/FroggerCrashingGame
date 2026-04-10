using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Enemy spawn")]
    [Tooltip("Prefab voor de vijand die van links naar rechts beweegt")]
    public GameObject enemyPrefab;

    [Range(0f, 1f)]
    [Tooltip("Kans dat er bij het maken van een nieuwe chunk een vijand verschijnt")]
    public float enemySpawnChance = 0.25f;

    [Tooltip("Snelheid waarmee de vijand naar rechts beweegt")]
    public float enemySpeed = 2f;

    [Header("Obstacle spawn")]
    public GameObject[] obstaclePrefabs;

    [Range(0, 10)]
    [Tooltip("Maximum number of obstacles to spawn per chunk")]
    public int maxObstaclesPerChunk = 2;

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

        // Check if player is on side lanes
        if (Mathf.Abs(player.position.x) > chunkWidth)
        {
            SceneManager.LoadScene("GameOver");
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

        // Eventueel een vijand spawnen op deze nieuwe chunk (zoals origineel)
        if (enemyPrefab != null && Random.value < enemySpawnChance)
        {
            Vector3 enemyPos = new Vector3(-chunkWidth, 0f, spawnZ);
            GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.Euler(0, 90, 90));
            activeChunks.Add(enemy);

            ObstacleMover mover = enemy.GetComponent<ObstacleMover>();
            if (mover == null)
                mover = enemy.AddComponent<ObstacleMover>();

            mover.speed = Mathf.Abs(enemySpeed);
            mover.leftBound = -chunkWidth * 2f;
            mover.rightBound = chunkWidth * 2f;
        }

        // Spawn obstacles on the road, random voor of achter
        if (obstaclePrefabs != null && obstaclePrefabs.Length > 0)
        {
            int numObstacles = Random.Range(0, maxObstaclesPerChunk + 1);
            for (int i = 0; i < numObstacles; i++)
            {
                GameObject obsPrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                float randomX = Random.Range(-chunkWidth / 2f, chunkWidth / 2f);

                // Kies random of obstakel voor of achter de chunk wordt gespawnd
                bool spawnInFront = (Random.value > 0.5f);
                float randomZ = spawnInFront ? spawnZ + Random.Range(0f, chunkLength) : spawnZ - Random.Range(0f, chunkLength);

                Vector3 obsPos = new Vector3(randomX, 0f, randomZ);
                GameObject obs = Instantiate(obsPrefab, obsPos, Quaternion.identity);
                obs.AddComponent<ObstacleTag>();
                activeChunks.Add(obs);
            }
        }

        spawnZ += chunkLength;   // Volgende chunk begint hier
    }
}
