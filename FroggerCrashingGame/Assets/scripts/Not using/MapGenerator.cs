using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 11;
    public int rows = 13;
    public float rowHeight = 1.0f;

    [Header("Lane Prefabs")]
    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject riverPrefab;
    public GameObject finishPrefab;

    [Header("Obstacle Prefabs")]
    public GameObject[] carPrefabs;
    public GameObject[] logPrefabs;

    [Header("Row Layout")]
    public RowDefinition[] rowDefinitions;

    [System.Serializable]
    public class RowDefinition
    {
        public LaneType laneType = LaneType.Grass;
        public float obstacleSpacing = 4.0f;
        public float obstacleChance = 0.6f;
        public float obstacleSpeed = 2.0f;
        public bool moveRight = true;
    }

    public enum LaneType
    {
        Grass,
        Road,
        River,
        Finish
    }

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (rowDefinitions == null || rowDefinitions.Length == 0)
        {
            Debug.LogWarning("MapGenerator: No rowDefinitions assigned.");
            return;
        }

        for (int row = 0; row < rows; row++)
        {
            RowDefinition rowDef = GetRowDefinition(row);
            Vector3 rowPosition = new Vector3(transform.position.x, transform.position.y + row * rowHeight, transform.position.z);

            GameObject lanePrefab = GetPrefabForLane(rowDef.laneType);
            if (lanePrefab != null)
            {
                Instantiate(lanePrefab, rowPosition, Quaternion.identity, transform);
            }

            if (rowDef.laneType == LaneType.Road)
            {
                SpawnObstacles(rowDef, rowPosition, carPrefabs);
            }
            else if (rowDef.laneType == LaneType.River)
            {
                SpawnObstacles(rowDef, rowPosition, logPrefabs);
            }
        }
    }

    RowDefinition GetRowDefinition(int rowIndex)
    {
        if (rowIndex < rowDefinitions.Length)
            return rowDefinitions[rowIndex];

        return rowDefinitions[rowDefinitions.Length - 1];
    }

    GameObject GetPrefabForLane(LaneType laneType)
    {
        switch (laneType)
        {
            case LaneType.Grass: return grassPrefab;
            case LaneType.Road: return roadPrefab;
            case LaneType.River: return riverPrefab;
            case LaneType.Finish: return finishPrefab;
            default: return grassPrefab;
        }
    }

    void SpawnObstacles(RowDefinition rowDef, Vector3 rowPosition, GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        float startX = transform.position.x - (width / 2f) + 0.5f;
        float endX = transform.position.x + (width / 2f) - 0.5f;
        float x = startX;

        while (x <= endX)
        {
            if (Random.value <= rowDef.obstacleChance)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                Vector3 obstaclePosition = new Vector3(x, rowPosition.y, rowPosition.z);
                GameObject obstacle = Instantiate(prefab, obstaclePosition, Quaternion.identity, transform);
                var mover = obstacle.GetComponent<ObstacleMover>();
                if (mover != null)
                {
                    mover.speed = rowDef.moveRight ? rowDef.obstacleSpeed : -rowDef.obstacleSpeed;
                }
            }

            x += rowDef.obstacleSpacing;
        }
    }
}
