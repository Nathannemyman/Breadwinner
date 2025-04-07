using UnityEngine;
using Unity.Cinemachine;

public class ScaleBackgroundWithCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void LateUpdate()
    {
        if (virtualCamera == null || spriteRenderer == null)
            return;

        float orthoSize = virtualCamera.Lens.OrthographicSize;
        float aspect = (float)Screen.width / Screen.height;

        // Calculate visible world dimensions
        float worldHeight = orthoSize * 2f;
        float worldWidth = worldHeight * aspect;

        // Get the size of the sprite in world units
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Calculate new scale to fill screen
        float scaleX = worldWidth / spriteSize.x;
        float scaleY = worldHeight / spriteSize.y;

        // Apply scale
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
