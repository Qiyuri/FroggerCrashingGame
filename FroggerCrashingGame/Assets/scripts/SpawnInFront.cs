using UnityEngine;
using System.Collections;

public class SpawnUsingReferenceX : MonoBehaviour
{
    public GameObject objectToSpawn;       // Het prefab dat je wilt spawnen
    public Transform playerTransform;      // De transform van de speler (voor z- en y-positie)
    public Transform referenceTransform;   // Het object waarvan je de x-positie wilt gebruiken
    public float distanceInFront = 5f;     // Afstand vooruit vanaf de speler (voor z-positie)
    public float minSpawnInterval = 1f;    // Minimale tijd tussen spawns (seconden)
    public float maxSpawnInterval = 3f;    // Maximale tijd tussen spawns (seconden)

    private bool spawning = false;

    void Update()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !spawning)
        {
            spawning = true;
            StartCoroutine(SpawnRoutine());
        }
        else if (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && spawning)
        {
            spawning = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawning)
        {
            SpawnObject();

            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn == null || playerTransform == null || referenceTransform == null)
        {
            Debug.LogWarning("ObjectToSpawn, PlayerTransform of ReferenceTransform is niet ingesteld.");
            return;
        }

        // Gebruik de x-positie van het referentieobject
        float spawnX = referenceTransform.position.x;

        // Gebruik z-positie van speler + afstand vooruit
        float spawnZ = playerTransform.position.z + distanceInFront;

        // Gebruik y-positie van speler (of pas aan indien nodig)
        float spawnY = playerTransform.position.y;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, spawnZ);

        Instantiate(objectToSpawn, spawnPos, playerTransform.rotation);
    }
}
