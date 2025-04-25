using UnityEngine;
using Unity.Cinemachine;

public class CinemachineZoomOnJump : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private BoxCollider2D boundaryCollider;
    [SerializeField] private float normalOrthoSize;
    [SerializeField] private float jumpOrthoSize;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float minimumOrthoSize = 3f;
    private CinemachineConfiner2D confiner;

    private bool isJumping = false;
    private bool wasFalling = false;


    void Start()
    {
        confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();

        if (confiner == null)
        {
            confiner = virtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
        }

        if (boundaryCollider != null)
        {
            confiner.BoundingShape2D = boundaryCollider;
            confiner.Damping = 0.2f; // Adjust as needed
        }

        // Ensure we're following the player
        virtualCamera.Follow = playerTransform;
    }

    void LateUpdate() // Using LateUpdate for smoother camera behavior
    {
        // Check player state
        bool currentlyJumping = playerRb.linearVelocity.y > 0.1f;
        bool currentlyFalling = playerRb.linearVelocity.y < -0.1f;

        if (currentlyJumping && !isJumping)
        {
            isJumping = true;
            wasFalling = false;
        }
        else if (currentlyFalling)
        {
            wasFalling = true;
        }
        else if (wasFalling && Mathf.Abs(playerRb.linearVelocity.y) < 0.1f)
        {
            isJumping = false;
            wasFalling = false;
        }

        // Determine target ortho size
        float targetOrthoSize = isJumping ? jumpOrthoSize : normalOrthoSize;

        // Calculate max safe ortho size based on bounds and player position
        float safeOrthoSize = CalculateSafeOrthoSize(targetOrthoSize);

        // Apply zoom with constraint
        var lens = virtualCamera.Lens;
        lens.OrthographicSize = Mathf.Lerp(
            lens.OrthographicSize,
            safeOrthoSize,
            Time.deltaTime * zoomSpeed
        );
        virtualCamera.Lens = lens;

        // Force update confiner to maintain player focus
        confiner.InvalidateBoundingShapeCache();
    }

    float CalculateSafeOrthoSize(float desiredSize)
    {
        if (boundaryCollider == null) return desiredSize;

        // Get camera aspect ratio
        float aspectRatio = Screen.width / (float)Screen.height;

        // Calculate camera width and height at desired ortho size
        float cameraHeight = desiredSize * 2; // Ortho size is half height
        float cameraWidth = cameraHeight * aspectRatio;

        // Get bounds info
        Bounds bounds = boundaryCollider.bounds;
        Vector2 boundsExtents = bounds.extents;

        // Calculate player's relative position within bounds (0-1 range)
        Vector2 playerPos = playerTransform.position;
        Vector2 boundsCenter = bounds.center;
        Vector2 playerPosRelative = new Vector2(
            (playerPos.x - boundsCenter.x) / boundsExtents.x,
            (playerPos.y - boundsCenter.y) / boundsExtents.y
        );

        // Clamp relative position to -1 to 1 range
        playerPosRelative.x = Mathf.Clamp(playerPosRelative.x, -1f, 1f);
        playerPosRelative.y = Mathf.Clamp(playerPosRelative.y, -1f, 1f);

        // Calculate available space in each direction
        float availSpaceLeft = boundsExtents.x * (1f + playerPosRelative.x);
        float availSpaceRight = boundsExtents.x * (1f - playerPosRelative.x);
        float availSpaceTop = boundsExtents.y * (1f + playerPosRelative.y);
        float availSpaceBottom = boundsExtents.y * (1f - playerPosRelative.y);

        // Calculate maximum possible size that fits within bounds
        float maxWidthFromAvailSpace = Mathf.Min(availSpaceLeft, availSpaceRight) * 2;
        float maxHeightFromAvailSpace = Mathf.Min(availSpaceTop, availSpaceBottom) * 2;

        // Convert to ortho size (half height)
        float maxWidthBasedOrthoSize = maxWidthFromAvailSpace / (2 * aspectRatio);
        float maxHeightBasedOrthoSize = maxHeightFromAvailSpace / 2;

        // Get the most constraining factor with a small margin
        float maxPossibleOrthoSize = Mathf.Min(maxWidthBasedOrthoSize, maxHeightBasedOrthoSize) * 0.95f;

        // Return the smallest of the desired and maximum possible sizes, but never smaller than minimum
        return Mathf.Max(minimumOrthoSize, Mathf.Min(desiredSize, maxPossibleOrthoSize));
    }
}