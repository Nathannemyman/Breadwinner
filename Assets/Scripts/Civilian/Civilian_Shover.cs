using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Civilian_Shover : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float upwardForce = 10f;
    [SerializeField] private float rotationAngle = 30f;
    [SerializeField] private float windupTime = 0.7f;
    [SerializeField] private float cooldownTime = 0.5f;
    [SerializeField] private float civilianAnimOffset = 0.2f; // Time before shove to start animation

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private int shakeVibrato = 20;

    private Animator animator;
    private Civilian_Movement civilianMovement;
    private PoliceMovement policeMovement;
    private bool canShove = true;
    private bool isPunching = false;
    private Coroutine activeCoroutine;
    private bool playerInTrigger = false;
    private GameObject currentPlayer = null;
    private PogoStickMovement playerPogoMovement = null;

    // Track shake and push states
    private bool isShaking = false;
    private bool hasPushed = false;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        civilianMovement = GetComponentInParent<Civilian_Movement>();
        policeMovement = GetComponentInParent<PoliceMovement>();

        if (civilianMovement == null && policeMovement == null)
        {
            Debug.LogWarning("Neither Civilian_Movement nor PoliceMovement component found in parent objects!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player detected entering trigger - canShove: {canShove}, activeCoroutine: {activeCoroutine != null}");

            // Find the PogoStickMovement component on the player
            playerPogoMovement = other.gameObject.GetComponent<PogoStickMovement>();
            if (playerPogoMovement == null)
            {
                Debug.LogWarning("PogoStickMovement component not found on player!");
            }

            if (canShove && activeCoroutine == null)
            {
                playerInTrigger = true;
                currentPlayer = other.gameObject;
                Debug.Log("Starting shove sequence");
                activeCoroutine = StartCoroutine(ShoveSequence(other.gameObject));
            }
            else
            {
                // Make sure we still track that player is in trigger even if we can't shove yet
                playerInTrigger = true;
                currentPlayer = other.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentPlayer)
        {
            Debug.Log($"Player exited trigger - isPunching: {isPunching}, isShaking: {isShaking}, hasPushed: {hasPushed}");
            playerInTrigger = false;

            // If we're still in windup phase, cancel the shove
            if (isPunching && !isShaking && !hasPushed)
            {
                if (activeCoroutine != null)
                {
                    Debug.Log("Canceling shove sequence due to player exit during windup");
                    StopCoroutine(activeCoroutine);
                    ResetState();
                    StartCoroutine(CooldownRoutine());
                }
            }
        }
    }
    private IEnumerator ShoveSequence(GameObject player)
    {
        Debug.Log("ShoveSequence started");

        // Get player components
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Transform playerTransform = player.transform;

        if (playerRb == null || playerTransform == null)
        {
            Debug.LogError("Player components missing, aborting shove");
            activeCoroutine = null;

            // Reset stunned state if interrupted
            if (playerPogoMovement != null)
            {
                playerPogoMovement.stunned = false;
            }

            yield break;
        }

        if (playerPogoMovement != null)
        {
            Debug.Log("Player stunned state set to true");
        }

        // Store original physics state for restoration later
        RigidbodyType2D originalBodyType = playerRb.bodyType;
        float originalGravityScale = playerRb.gravityScale;

        // Safety flag to track if we need emergency physics restoration
        bool needPhysicsRestoration = true;

        // Setup state
        isPunching = true;
        canShove = false;
        isShaking = false;
        hasPushed = false;

        // Stop the character from moving BUT DO NOT FREEZE ANIMATION
        StopCharacterMovement(true); // Pass true to indicate we're in windup phase

        Debug.Log("Windup phase started");

        // STEP 1: Wait for windup time (civilian stands still)
        yield return new WaitForSeconds(windupTime);

        // Check player validity
        if (player == null || playerRb == null || !playerInTrigger)
        {
            // Reset stunned state if interrupted
            if (playerPogoMovement != null)
            {
                playerPogoMovement.stunned = false;
            }

            ResetState();
            StartCoroutine(CooldownRoutine());
            yield break;
        }

        // STEP 2: Start the animation IMMEDIATELY after windup
        float animationStartTime = Time.time;
        float minimumAnimationDuration = 0.55f; // Set this to match your full animation length

        if (animator != null)
        {
            // Make sure animator is running at normal speed
            animator.speed = 1;

            // Set the animation parameter
            animator.SetBool("IsPunching", true);

            Debug.Log("Starting punch animation immediately after windup");
        }

        // STEP 3: Wait for civilianAnimOffset seconds while animation plays
        if (civilianAnimOffset > 0)
        {
            Debug.Log($"Animation playing for {civilianAnimOffset} seconds before physical impact");
            yield return new WaitForSeconds(civilianAnimOffset);
        }

        // Check if player is still valid before physical effects
        if (player == null || playerRb == null || !playerInTrigger)
        {
            Debug.Log("Player no longer valid before shake");

            // Reset stunned state if interrupted
            if (playerPogoMovement != null)
            {
                playerPogoMovement.stunned = false;
            }

            // Ensure animation completes before resetting
            float timeElapsed = Time.time - animationStartTime;
            if (timeElapsed < minimumAnimationDuration)
            {
                yield return new WaitForSeconds(minimumAnimationDuration - timeElapsed);
            }

            ResetState();
            StartCoroutine(CooldownRoutine());
            yield break;
        }

        // Store original position for shake effect
        Vector3 originalPosition = playerTransform.position;

        // Freeze player temporarily for the shake
        playerRb.linearVelocity = Vector2.zero;
        playerRb.bodyType = RigidbodyType2D.Kinematic;

        // STEP 4: Perform shake
        playerPogoMovement.stunned = true;
        Debug.Log("Shake phase started");
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            if (player == null || playerTransform == null)
            {
                Debug.LogError("Player lost during shake phase");

                // Reset stunned state if interrupted
                if (playerPogoMovement != null)
                {
                    playerPogoMovement.stunned = false;
                }

                break;
            }

            elapsed += Time.deltaTime;
            float progress = elapsed / shakeDuration;
            float amplitude = shakeIntensity * (1f - progress);

            float xOffset = Mathf.Sin(progress * shakeVibrato * 6.28f) * amplitude;
            float yOffset = Mathf.Cos(progress * shakeVibrato * 6.28f) * amplitude * 0.7f;

            playerTransform.position = originalPosition + new Vector3(xOffset, yOffset, 0);

            yield return null;
        }

        isShaking = false;
        Debug.Log("Shake phase completed");

        // Reset position after shake
        if (playerTransform != null)
        {
            playerTransform.position = originalPosition;
        }

        // Wait to ensure physics is ready
        yield return new WaitForFixedUpdate();

        // Check if player is still valid before pushing
        if (player == null || playerRb == null)
        {
            Debug.LogError("Player lost before push phase");

            // Reset stunned state if interrupted
            if (playerPogoMovement != null)
            {
                playerPogoMovement.stunned = false;
            }

            // We'll do physics restoration in the cleanup phase
            goto CleanupPhase;
        }

        // STEP 5: Unfreeze player for the push and restore physics
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        playerRb.gravityScale = originalGravityScale;
        needPhysicsRestoration = false;  // We've successfully restored physics

        // Apply the push force based on officer's rotation
        Debug.Log("Push phase started");
        hasPushed = true;

        // Get the parent transform (where the rotation is set)
        Transform parentTransform = transform.parent;

        // Determine facing direction based on Y rotation
        bool isFacingLeft = false; // Default facing right
        if (parentTransform != null)
        {
            // Check Y rotation - 0 is facing right, 180 is facing left in the provided script
            float yRotation = parentTransform.rotation.eulerAngles.y;
            isFacingLeft = Mathf.Approximately(yRotation, 180f);
            Debug.Log($"Officer rotation: {yRotation} degrees, isFacingLeft: {isFacingLeft}");
        }

        // Set direction value based on facing direction
        float directionValue = isFacingLeft ? 1 : -1;

        // Calculate push force
        float xForce = directionValue * pushForce;
        Vector2 finalForce = new Vector2(xForce, upwardForce);

        Debug.Log($"FORCE CALCULATION: pushForce={pushForce}, direction={directionValue}, xForce={xForce}");
        Debug.Log($"Final force vector: x={finalForce.x}, y={finalForce.y}");

        playerRb.linearVelocity = Vector2.zero; // Reset velocity before push
        playerRb.AddForce(finalForce, ForceMode2D.Impulse);

        // Apply rotation if possible - flip the rotation based on direction
        if (playerPogoMovement != null)
        {
            float flippedRotation = directionValue * rotationAngle;
            Debug.Log($"Applying rotation: {flippedRotation} (base angle: {rotationAngle}, direction: {directionValue})");
            playerPogoMovement.ApplyExternalRotation(flippedRotation);

            // Change layer to Everything_But_Ground when player is shoved
            playerPogoMovement.ChangeLayerRecursively(player, LayerMask.NameToLayer("Everything_But_Ground"));

            // Start coroutine to reset layer back to Default after 2 seconds
            StartCoroutine(ResetLayerAfterDelay(player));
        }

        // Calculate how much time has passed since animation started
        float animationElapsedTime = Time.time - animationStartTime;

        // Wait additional time to ensure the full animation plays
        if (animationElapsedTime < minimumAnimationDuration)
        {
            float remainingTime = minimumAnimationDuration - animationElapsedTime;
            Debug.Log($"Waiting {remainingTime} seconds to complete animation");
            yield return new WaitForSeconds(remainingTime);
        }

    // Cleanup phase label
    CleanupPhase:

        // SAFETY CHECK: Restore player physics if needed and player still exists
        if (needPhysicsRestoration && player != null && playerRb != null)
        {
            Debug.Log("Safety physics restoration during cleanup");
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            playerRb.gravityScale = originalGravityScale;
        }

        // Reset stunned state to false at the end of the sequence
        if (playerPogoMovement != null)
        {
            playerPogoMovement.stunned = false;
            Debug.Log("Player stunned state set to false");
        }

        // Calculate final animation time
        float finalAnimationTime = Time.time - animationStartTime;
        Debug.Log($"Animation played for a total of {finalAnimationTime} seconds");

        // Reset state
        ResetState();

        // Start cooldown
        StartCoroutine(CooldownRoutine());
    }
    // New coroutine to reset the layer after 1 second
    private IEnumerator ResetLayerAfterDelay(GameObject player)
    {
        yield return new WaitForSeconds(1f);

        if (player != null && playerPogoMovement != null)
        {
            playerPogoMovement.ChangeLayerRecursively(player, LayerMask.NameToLayer("Default"));
            Debug.Log("Player layer reset to Default after being shoved");
        }
    }

    // Add this new function to handle emergency physics restoration
    public void EmergencyRestorePlayerPhysics()
    {
        if (currentPlayer != null)
        {
            Rigidbody2D playerRb = currentPlayer.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.bodyType == RigidbodyType2D.Kinematic)
            {
                Debug.Log("Emergency: Restoring player physics state!");
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                // Default gravity scale is 1, but you might want to test different values
                playerRb.gravityScale = 1f;
            }
        }
    }


    private void StopCharacterMovement(bool isWindupPhase = false)
    {
        // Stop Civilian movement if available
        if (civilianMovement != null)
        {
            // Access the Civilian_Movement's fields using reflection
            System.Reflection.FieldInfo movingField =
                typeof(Civilian_Movement).GetField("isMoving",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (movingField != null)
            {
                movingField.SetValue(civilianMovement, false);

                // Only pause animation if we're not in windup phase
                if (animator != null && !isWindupPhase)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    animator.speed = 0;
                    animator.Play(stateInfo.fullPathHash, 0, 0.0f);
                }
            }
        }

        // Stop Police movement if available - trigger punch animation
        if (policeMovement != null)
        {
            // Access the PoliceMovement's fields using reflection
            System.Reflection.FieldInfo movingField =
                typeof(PoliceMovement).GetField("isMoving",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (movingField != null)
            {
                movingField.SetValue(policeMovement, false);

                // For police, we don't freeze animation
                // If in windup phase, we wait to set the punch animation
                if (animator != null && !isWindupPhase)
                {
                    animator.SetBool("IsPunching", true);
                    animator.speed = 1;
                }
            }
        }
    }

    private void ResumeCivilianMovement()
    {
        // Resume Civilian movement if available
        if (civilianMovement != null)
        {
            System.Reflection.FieldInfo movingField =
                typeof(Civilian_Movement).GetField("isMoving",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (movingField != null)
            {
                movingField.SetValue(civilianMovement, true);

                // No need to reset animation here since already done in ResetState
            }
        }

        // Resume Police movement if available
        if (policeMovement != null)
        {
            System.Reflection.FieldInfo movingField =
                typeof(PoliceMovement).GetField("isMoving",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (movingField != null)
            {
                movingField.SetValue(policeMovement, true);

                // Animation already reset in ResetState
            }
        }
    }

    private IEnumerator CooldownRoutine()
    {
        Debug.Log("Cooldown started");
        yield return new WaitForSeconds(cooldownTime);
        canShove = true;
        Debug.Log("Cooldown completed, can shove again");

        // Check if player is still in trigger after cooldown
        if (playerInTrigger && currentPlayer != null && activeCoroutine == null)
        {
            Debug.Log("Player still in trigger after cooldown - initiating new shove sequence");
            activeCoroutine = StartCoroutine(ShoveSequence(currentPlayer));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // This ensures we can detect the player if they're already in the trigger when cooldown ends
        if (other.CompareTag("Player") && canShove && activeCoroutine == null && !playerInTrigger)
        {
            Debug.Log("Player detected in OnTriggerStay2D - starting shove sequence");
            playerInTrigger = true;
            currentPlayer = other.gameObject;
            activeCoroutine = StartCoroutine(ShoveSequence(other.gameObject));
        }
    }

    private void ResetState()
    {
        isPunching = false;
        isShaking = false;
        hasPushed = false;
        EmergencyRestorePlayerPhysics();

        // Ensure animation is properly reset before resuming movement
        if (animator != null)
        {
            animator.SetBool("IsPunching", false);
            animator.speed = 1;  // Make sure animation speed is normal

            // Force a small frame update to ensure animation transitions correctly
            if (animator.enabled)
            {
                animator.Update(0.01f);
            }
        }

        // Resume character movement
        ResumeCivilianMovement();
        Debug.Log("State reset, character movement resumed");

        activeCoroutine = null;
    }

    private void OnDisable()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        // Emergency restore before disabling
        EmergencyRestorePlayerPhysics();

        ResetState();
        canShove = true;
    }
}