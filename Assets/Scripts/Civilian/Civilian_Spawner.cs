using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject civilianPrefab;
    public GameObject policeOfficerPrefab;
    public GameObject deathBlastPrefab;
    public Camera targetCamera;
    public float ySpawnLevel = 0f;
    public float offscreenOffset = 2f;
    public Timer timer;
    public float despawnDistance = 40f; // How far to the left of the camera before the enemies despawn

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

        // Check for entities that have fallen below y=-10
        CheckForFallenEntities();
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

        if (GameManager.Instance != null && GameManager.Instance.HasBread)
        {
            // Spawn on left side when HasBread is true
            float leftEdgeX = targetCamera.transform.position.x - (cameraWidth / 2f);
            return new Vector3(
                leftEdgeX - offscreenOffset,
                ySpawnLevel,
                0f
            );
        }
        else
        {
            // Original spawn on right side
            float rightEdgeX = targetCamera.transform.position.x + (cameraWidth / 2f);
            return new Vector3(
                rightEdgeX + offscreenOffset,
                ySpawnLevel,
                0f
            );
        }
    }

    void CleanupOffscreenEntities()
    {
        float leftBoundary = targetCamera.transform.position.x - (targetCamera.orthographicSize * targetCamera.aspect) - despawnDistance;
        float rightBoundary = targetCamera.transform.position.x + (targetCamera.orthographicSize * targetCamera.aspect) + 30f;

        // Check each entity and remove those that are too far left or right
        for (int i = spawnedEntities.Count - 1; i >= 0; i--)
        {
            GameObject entity = spawnedEntities[i];

            // Skip if entity was already destroyed
            if (entity == null)
            {
                spawnedEntities.RemoveAt(i);
                continue;
            }

            // Check if entity is too far left or too far right
            if (entity.transform.position.x < leftBoundary || entity.transform.position.x > rightBoundary)
            {
                Destroy(entity);
                spawnedEntities.RemoveAt(i);
            }
        }
    }

    void CheckForFallenEntities()
    {
        // Check each entity to see if it has fallen below y=-10
        for (int i = spawnedEntities.Count - 1; i >= 0; i--)
        {
            GameObject entity = spawnedEntities[i];

            // Skip if entity was already destroyed
            if (entity == null)
            {
                spawnedEntities.RemoveAt(i);
                continue;
            }

            // Check if entity has fallen below y=-10
            if (entity.transform.position.y < -10f)
            {
                // Get the entity's x position
                float xPosition = entity.transform.position.x;

                // Get the entity's velocity direction
                Vector2 velocityDirection = Vector2.right; // Default direction

                // Try to get the Rigidbody2D component
                Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
                if (rb != null && rb.linearVelocity.magnitude > 0.1f)
                {
                    velocityDirection = rb.linearVelocity.normalized;
                }
                else
                {
                    // If no Rigidbody2D or no significant velocity, infer direction from spawn position
                    bool hasBreak = GameManager.Instance != null && GameManager.Instance.HasBread;
                    velocityDirection = hasBreak ? Vector2.right : Vector2.left;
                }

                // Spawn the Death_Blast prefab
                SpawnDeathBlast(xPosition, velocityDirection);

                // Remove the entity
                Destroy(entity);
                spawnedEntities.RemoveAt(i);
            }
        }
    }

    void SpawnDeathBlast(float xPosition, Vector2 direction)
    {
        if (deathBlastPrefab == null)
        {
            Debug.LogWarning("Death Blast prefab not assigned!");
            return;
        }

        // Calculate rotation to face the direction with a 90-degree offset
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Create the Death_Blast at the x position and y=-10
        Vector3 spawnPosition = new Vector3(xPosition, -10f, 0f);
        GameObject deathBlast = Instantiate(deathBlastPrefab, spawnPosition, rotation);

        // Start coroutine to despawn after 2 seconds
        StartCoroutine(DespawnAfterDelay(deathBlast, 2.0f));
    }

    IEnumerator DespawnAfterDelay(GameObject obj, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy the object if it still exists
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}