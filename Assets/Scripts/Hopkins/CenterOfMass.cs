using UnityEngine;

public class CenterOfGravityController : MonoBehaviour
{
    [SerializeField] private Vector2 centerOfGravity = Vector2.zero;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        UpdateCenterOfGravity();
    }

    private void OnValidate()
    {
        if (rb != null)
        {
            UpdateCenterOfGravity();
        }
    }

    private void UpdateCenterOfGravity()
    {
        rb.centerOfMass = centerOfGravity;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && !enabled)
            return;

        // Get the transform to properly position the gizmo
        Transform t = transform;
        Vector3 worldCogPosition = t.TransformPoint(centerOfGravity);

        // Draw the center of gravity marker
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldCogPosition, 0.1f);

        // Draw a line connecting to the object's pivot
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(t.position, worldCogPosition);
    }
}