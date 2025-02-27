using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // The enemy prefab
    public float spawnInterval = 2f; // Time between spawns
    public Camera targetCamera; // Reference to the camera
    public float ySpawnLevel = 0f; // Y level for enemy spawning
    public float offscreenOffset = 2f; // How far off-screen to spawn

    private void Start()
    {
        // Validate camera reference
        if (targetCamera == null)
        {
            Debug.LogError("Target camera not assigned to EnemySpawner!");
            return;
        }

        // Start spawning enemies
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (targetCamera == null) return;

        // Calculate spawn position to the right of camera view
        float cameraHeight = 2f * targetCamera.orthographicSize;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // Calculate right edge of camera view
        float rightEdgeX = targetCamera.transform.position.x + (cameraWidth / 2f);

        // Spawn position: right of camera view plus offset
        Vector3 spawnPosition = new Vector3(
            rightEdgeX + offscreenOffset,
            ySpawnLevel,
            0f
        );

        // Instantiate the enemy prefab at the spawn position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        if (targetCamera == null) return;

        // Draw spawn area visualization in the scene view
        Gizmos.color = Color.red;

        // Calculate right edge of camera view
        float cameraHeight = 2f * targetCamera.orthographicSize;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        float rightEdgeX = targetCamera.transform.position.x + (cameraWidth / 2f);

        // Draw a line showing where enemies will spawn
        Vector3 lineStart = new Vector3(rightEdgeX + offscreenOffset, ySpawnLevel - 1f, 0f);
        Vector3 lineEnd = new Vector3(rightEdgeX + offscreenOffset, ySpawnLevel + 1f, 0f);
        Gizmos.DrawLine(lineStart, lineEnd);

        // Draw a sphere at the spawn point
        Vector3 spawnPosition = new Vector3(rightEdgeX + offscreenOffset, ySpawnLevel, 0f);
        Gizmos.DrawSphere(spawnPosition, 0.5f);
    }
}