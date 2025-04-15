using UnityEngine;

public class PartialCenterOfMassModifier : MonoBehaviour
{
    [Range(0f, 1f)]
    public float customMassPercentage = 0.7f; // 70% at custom point, 30% at default
    public Vector2 customMassPosition = new Vector2(0, 0);

    private Rigidbody2D rb;
    private Vector2 defaultCenterOfMass;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Store the default center of mass
            defaultCenterOfMass = rb.centerOfMass;

            // Calculate and set the weighted center of mass
            UpdateCenterOfMass();
        }
    }

    void Update()
    {
        // Allow for runtime adjustments
        if (rb != null)
        {
            UpdateCenterOfMass();
        }
    }

    void UpdateCenterOfMass()
    {
        // Calculate weighted average of the two positions
        Vector2 weightedCenterOfMass = (customMassPosition * customMassPercentage) +
                                       (defaultCenterOfMass * (1f - customMassPercentage));

        rb.centerOfMass = weightedCenterOfMass;
    }

    // Visualize the center of mass
    void OnDrawGizmosSelected()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && Application.isPlaying)
        {
            // Draw the actual center of mass
            Gizmos.color = Color.red;
            Vector3 comPosition = transform.position + (Vector3)(transform.rotation * rb.centerOfMass);
            Gizmos.DrawSphere(comPosition, 0.1f);

            // Draw the custom position point
            Gizmos.color = Color.blue;
            Vector3 customPosition = transform.position + (Vector3)(transform.rotation * customMassPosition);
            Gizmos.DrawSphere(customPosition, 0.08f);

            // Draw the default position point
            Gizmos.color = Color.green;
            Vector3 defaultPosition = transform.position + (Vector3)(transform.rotation * defaultCenterOfMass);
            Gizmos.DrawSphere(defaultPosition, 0.08f);
        }
    }
}