using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cringe_Speaker_Blast : MonoBehaviour
{
    [Header("Blast Settings")]
    [SerializeField] private float blastForce = 20f;
    [SerializeField] private float rotationForce = 30f;

    [Header("Status")]
    [SerializeField] private bool cringeSpeakerEnabled = false; // Set this in the inspector

    // Reference to EnemySpawner script
    private EnemySpawner enemySpawner;

    // Reference to parent's rigidbody
    private Rigidbody2D parentRb;

    // Layer that blasted objects should be moved to
    [SerializeField] private string blastedLayerName = "Ignore Raycast"; // Default Unity layer that ignores physics
    private int blastedLayer;

    private void Start()
    {
        // Find EnemySpawner script in the scene
        enemySpawner = FindObjectOfType<EnemySpawner>();

        // Get parent's rigidbody
        if (transform.parent != null)
        {
            parentRb = transform.parent.GetComponent<Rigidbody2D>();
        }

        // Get the layer to use for blasted objects
        blastedLayer = LayerMask.NameToLayer(blastedLayerName);

        // Debug log for Cringe Speaker status in GameData
        Debug.Log("!!!!!!!!!!!Cringe Speaker status: " + GameData.Instance.HasItem(CollectableType.CringeSpeaker) + " !!!!!!!!!!!!!!!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check conditions
        if (!cringeSpeakerEnabled) return;
        if (enemySpawner == null || !enemySpawner.cringeSpeakerActive || !GameData.Instance.HasItem(CollectableType.CringeSpeaker)) return;

        // Check if colliding object has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            BlastParentAway(collision.transform.position, playerRb);
        }
    }

    private void BlastParentAway(Vector2 playerPosition, Rigidbody2D playerRb)
    {
        if (parentRb == null || transform.parent == null) return;

        // Get all colliders from the parent and children
        DisableAllInteractions(transform.parent);

        // Make rigidbody dynamic with no constraints
        parentRb.bodyType = RigidbodyType2D.Dynamic;
        parentRb.constraints = RigidbodyConstraints2D.None;

        // Move to non-interacting layer
        MoveToNonInteractingLayer(transform.parent);

        Vector2 blastDirection;

        // Check if player is moving
        if (playerRb != null && playerRb.linearVelocity.magnitude > 0.1f)
        {
            // Shoot in the SAME direction as player's velocity
            blastDirection = playerRb.linearVelocity.normalized;
        }
        else
        {
            // Fallback: shoot TOWARD player's position
            blastDirection = (playerPosition - (Vector2)transform.parent.position).normalized;
        }

        // Apply force and rotation
        parentRb.AddForce(blastDirection * blastForce, ForceMode2D.Impulse);
        parentRb.AddTorque(rotationForce, ForceMode2D.Impulse);

        // Tag the object as blasted
        transform.parent.gameObject.tag = "Untagged";

        // Destroy all scripts except those needed for visuals
        DestroyAllScriptsExceptVisuals(transform.parent);
    }

    private void DisableAllInteractions(Transform targetTransform)
    {
        // Disable all colliders in the hierarchy
        Collider2D[] colliders = targetTransform.GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Disable any other physics components
        Joint2D[] joints = targetTransform.GetComponentsInChildren<Joint2D>(true);
        foreach (Joint2D joint in joints)
        {
            joint.enabled = false;
        }

        // Disable any AI controllers
        MonoBehaviour[] behaviors = targetTransform.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behavior in behaviors)
        {
            // Don't disable this script or essential components
            if (behavior == this || behavior is SpriteRenderer || behavior is Rigidbody2D)
                continue;

            behavior.enabled = false;
        }
    }

    private void MoveToNonInteractingLayer(Transform targetTransform)
    {
        // Skip if layer is invalid
        if (blastedLayer < 0) return;

        // Change layer of all objects in hierarchy
        targetTransform.gameObject.layer = blastedLayer;
        foreach (Transform child in targetTransform)
        {
            MoveToNonInteractingLayer(child);
        }
    }

    private void DestroyAllScriptsExceptVisuals(Transform targetTransform)
    {
        // Get all components
        Component[] components = targetTransform.GetComponents<Component>();

        foreach (Component component in components)
        {
            // Don't destroy essential components
            if (component is Transform || component is Rigidbody2D ||
                component is SpriteRenderer || component is Animator)
                continue;

            // Don't destroy this script
            if (component == this)
                continue;

            // Destroy all other components
            if (component is MonoBehaviour)
            {
                Destroy(component);
            }
        }

        // Do the same for all children
        foreach (Transform child in targetTransform)
        {
            if (child != transform) // Don't process this object
            {
                DestroyAllScriptsExceptVisuals(child);
            }
        }
    }
}