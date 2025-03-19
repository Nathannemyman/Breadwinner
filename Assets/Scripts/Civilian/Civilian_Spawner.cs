using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject civilianPrefab;
    public GameObject policeOfficerPrefab;
    public float civilianSpawnInterval = 2f;
    public float policeSpawnInterval = 3f;
    public Camera targetCamera;
    public float ySpawnLevel = 0f;
    public float offscreenOffset = 2f;

    private bool hasStartedPoliceSpawning = false;

    private void Start()
    {
        // Start spawning civilians
        InvokeRepeating(nameof(SpawnEnemy), 0f, civilianSpawnInterval);
    }

    private void Update()
    {
        // Check if police spawning should be occur
        if (GameManager.Instance.policeSpawning && !hasStartedPoliceSpawning)
        {
            hasStartedPoliceSpawning = true;
            // Start spawning police officers
            InvokeRepeating(nameof(SpawnPoliceOfficer), 0f, policeSpawnInterval);
        }
    }

    void SpawnEnemy()
    {
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
        // Instantiate(civilianPrefab, spawnPosition, Quaternion.identity);
    }

    void SpawnPoliceOfficer()
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

        // Instantiate the police officer prefab at the spawn position
        Instantiate(policeOfficerPrefab, spawnPosition, Quaternion.identity);
    }


}