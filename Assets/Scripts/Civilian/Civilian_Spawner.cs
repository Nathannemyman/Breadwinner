using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject civilianPrefab;
    public GameObject policeOfficerPrefab;
    public Camera targetCamera;
    public float ySpawnLevel = 0f;
    public float offscreenOffset = 2f;
    public Timer timer;
    public float despawnDistance = 30f; // How far to the left of the camera before the enemies despawn

    // Spawn intervals
    public float startCivilianInterval = 0.05f;
    public float endCivilianInterval = 10f;
    public float startPoliceInterval = 10f;
    public float endPoliceInterval = 1f;

    private float totalGameTime = 300f;
    private float currentCivilianInterval;
    private float currentPoliceInterval;
    private float timeSinceLastCivilianSpawn = 0f;
    private float timeSinceLastPoliceSpawn = 0f;

    // Switch for police spawning
    private bool policeSpawningActivated = false;

    // List to track spawned objects
    private List<GameObject> spawnedEntities = new List<GameObject>();

    private void Start()
    {
        // Initialize with default value
        totalGameTime = 300f;

        // Try to get time from timer if available
        if (timer != null)
        {
            // Wait one frame to ensure timer has initialized
            StartCoroutine(InitializeAfterTimer());
        }

    }

    private IEnumerator InitializeAfterTimer()
    {
        // Wait for end of frame to ensure timer Start() has run
        yield return new WaitForEndOfFrame();

        if (timer != null)
        {
            totalGameTime = timer.TimeRemaining;
            Debug.Log($"[EnemySpawner] InitializeAfterTimer - totalGameTime set to: {totalGameTime}");
        }
    }


    private void Update()
    {
        // Add null check and safety for timeRatio calculation
        float timeRatio = 1.0f;
        if (timer != null && totalGameTime > 0)
        {
            timeRatio = Mathf.Clamp01(timer.TimeRemaining / totalGameTime);

        }
        currentCivilianInterval = Mathf.Lerp(endCivilianInterval, startCivilianInterval, timeRatio);
        currentPoliceInterval = Mathf.Lerp(endPoliceInterval, startPoliceInterval, timeRatio);

        // Civilian spawning
        timeSinceLastCivilianSpawn += Time.deltaTime;
        if (timeSinceLastCivilianSpawn >= currentCivilianInterval)
        {
            SpawnEnemy();
            timeSinceLastCivilianSpawn = 0f;
        }

        // Check if police spawning should be activated - once activated, it stays on
        if (GameManager.Instance != null && GameManager.Instance.policeSpawning)
        {
            policeSpawningActivated = true;
        }

        // Police spawning based on one-way switch
        if (policeSpawningActivated)
        {
            timeSinceLastPoliceSpawn += Time.deltaTime;
            if (timeSinceLastPoliceSpawn >= currentPoliceInterval)
            {
                SpawnPoliceOfficer();
                timeSinceLastPoliceSpawn = 0f;
            }
        }

        // Clean up entities that have moved off-screen
        CleanupOffscreenEntities();
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = CalculateSpawnPosition();
        GameObject civilian = Instantiate(civilianPrefab, spawnPosition, Quaternion.identity);
        spawnedEntities.Add(civilian);
    }

    void SpawnPoliceOfficer()
    {
        Vector3 spawnPosition = CalculateSpawnPosition();
        GameObject police = Instantiate(policeOfficerPrefab, spawnPosition, Quaternion.identity);
        spawnedEntities.Add(police);
    }

    Vector3 CalculateSpawnPosition()
    {
        float cameraHeight = 2f * targetCamera.orthographicSize;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        float rightEdgeX = targetCamera.transform.position.x + (cameraWidth / 2f);

        return new Vector3(
            rightEdgeX + offscreenOffset,
            ySpawnLevel,
            0f
        );
    }

    void CleanupOffscreenEntities()
    {
        float leftBoundary = targetCamera.transform.position.x - (targetCamera.orthographicSize * targetCamera.aspect) - despawnDistance;

        // Check each entity and remove those that are too far left
        for (int i = spawnedEntities.Count - 1; i >= 0; i--)
        {
            GameObject entity = spawnedEntities[i];

            // Skip if entity was already destroyed
            if (entity == null)
            {
                spawnedEntities.RemoveAt(i);
                continue;
            }

            // Check if entity is too far left
            if (entity.transform.position.x < leftBoundary)
            {
                Destroy(entity);
                spawnedEntities.RemoveAt(i);
            }
        }
    }
}