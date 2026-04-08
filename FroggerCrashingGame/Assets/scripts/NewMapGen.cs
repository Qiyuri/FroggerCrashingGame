using UnityEngine;

public class NewMapGen : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapLength = 100; // Length of the map in units
    public float roadWidth = 10f;
    public float segmentLength = 5f; // Length of each road segment

    [Header("Prefabs")]
    public GameObject roadPrefab;
    public GameObject frogPrefab;
    public GameObject obstaclePrefab; // Optional obstacles

    [Header("Frog Spawning")]
    public int frogsPerSegment = 3;
    public float frogSpawnHeight = 0.5f;
    public float frogSpeed = 2f;
    public float frogJumpInterval = 1f;

    [Header("Obstacles")]
    public float obstacleChance = 0.2f; // Chance to spawn obstacle per segment

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        if (roadPrefab == null)
        {
            Debug.LogError("Road prefab not assigned!");
            return;
        }

        if (frogPrefab == null)
        {
            Debug.LogWarning("Frog prefab not assigned. Frogs will not spawn.");
        }

        Vector3 currentPosition = transform.position;

        for (int i = 0; i < mapLength / segmentLength; i++)
        {
            // Generate road segment
            GameObject roadSegment = Instantiate(roadPrefab, currentPosition, Quaternion.identity, transform);
            roadSegment.transform.localScale = new Vector3(roadWidth, 1f, segmentLength);

            // Spawn frogs on this segment
            if (frogPrefab != null)
            {
                SpawnFrogsOnSegment(currentPosition, segmentLength, roadWidth);
            }

            // Spawn obstacles occasionally
            if (obstaclePrefab != null && Random.value < obstacleChance)
            {
                SpawnObstacleOnSegment(currentPosition, segmentLength, roadWidth);
            }

            // Move to next segment
            currentPosition.z += segmentLength;
        }
    }

    private void SpawnFrogsOnSegment(Vector3 segmentStart, float length, float width)
    {
        for (int i = 0; i < frogsPerSegment; i++)
        {
            // Random position within the segment
            float x = Random.Range(segmentStart.x - width/2 + 1f, segmentStart.x + width/2 - 1f);
            float z = Random.Range(segmentStart.z, segmentStart.z + length);

            Vector3 frogPosition = new Vector3(x, frogSpawnHeight, z);

            GameObject frog = Instantiate(frogPrefab, frogPosition, Quaternion.identity, transform);

            // Add movement to frog if it has a component
            FrogController frogController = frog.GetComponent<FrogController>();
            if (frogController != null)
            {
                frogController.speed = frogSpeed;
                frogController.jumpInterval = frogJumpInterval;
            }
        }
    }

    private void SpawnObstacleOnSegment(Vector3 segmentStart, float length, float width)
    {
        float x = Random.Range(segmentStart.x - width/2 + 1f, segmentStart.x + width/2 - 1f);
        float z = Random.Range(segmentStart.z, segmentStart.z + length);

        Vector3 obstaclePosition = new Vector3(x, 0.5f, z);

        Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, transform);
    }

    // Optional: Regenerate map when needed
    public void RegenerateMap()
    {
        // Destroy existing map
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Generate new map
        GenerateMap();
    }
}
