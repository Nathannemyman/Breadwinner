using System.Collections;
using UnityEngine;

public class PoliceMovement : MonoBehaviour
{
    private PoliceShooter policeShooter;
    private Transform playerPosition;
    private float speed;
    private bool isMoving = true;
    private const float check_movement_interval = 3f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isJumping = false;
    private float jumpForce = 1200f;
    private int groundLayerMask;
    private float despawnDistance = 100f;
    private float despawnCheckInterval = 2f;
    private Animator animator; // Animator reference

    private float lastTurnTime = 0f;
    private float turnCooldown = 1f; // Police can't turn around more than once per second
    private int lastFacingDirection = 0; // 0 for none, 1 for right, -1 for left

    void Start()
    {
        policeShooter = GetComponent<PoliceShooter>();
        animator = GetComponent<Animator>(); // Initialize animator
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");

        DeathCollision deathCollision = GetComponentInChildren<DeathCollision>();
        deathCollision.isPoliceOfficer = true;

        FindPlayerReference();

        speed = Random.Range(2f, 4f);
        StartCoroutine(Movement_Stops_Routine());

        StartCoroutine(CheckDistanceForDespawn());

        lastFacingDirection = spriteRenderer.flipX ? -1 : 1;

        if (GameManager.Instance.HasBread)
        {
            // Face right (flipped sprite)
            transform.rotation = Quaternion.Euler(0, 180, 0);
            lastFacingDirection = 1; // Right
        }
        else
        {
            // Default facing left
            transform.rotation = Quaternion.Euler(0, 0, 0);
            lastFacingDirection = -1; // Left
        }
    }

    void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerPosition = playerObj.transform;
    }

    IEnumerator CheckDistanceForDespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(despawnCheckInterval);

            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition.position);

            if (distanceToPlayer > despawnDistance)
            {
                Debug.Log($"Police despawning - distance to player: {distanceToPlayer}");
                Destroy(gameObject);
                yield break;
            }
        }
    }

    void Update()
    {
        // Check if player is above Y=3 and make police officer jump
        if (playerPosition != null && playerPosition.position.y > 3f)
        {
            if (!isJumping && IsGrounded())
            {
                Jump();
            }
        }

        // Handle animation state based on movement
        if (isMoving)
        {
            animator.speed = 1; // Resume animation while moving
        }

        // Update xVelocity parameter in animator
        if (animator != null && rb != null)
        {
            animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void FixedUpdate()
    {
        // We've moved the animation parameter update to Update() for consistency

        if (isMoving && playerPosition != null)
        {
            // Determine direction based on player position
            float directionToPlayer = playerPosition.position.x - transform.position.x;
            int newFacingDirection = Mathf.Sign(directionToPlayer) > 0 ? 1 : -1;

            // Check if this is a turn (direction change)
            bool isTurning = newFacingDirection != lastFacingDirection;

            // Check if enough time has passed since the last turn
            bool canTurn = Time.time - lastTurnTime >= turnCooldown;

            // Apply horizontal movement
            Vector2 velocity = rb.linearVelocity;

            if (isTurning)
            {
                if (canTurn)
                {
                    // Allow the turn
                    velocity.x = newFacingDirection * speed;
                    transform.rotation = Quaternion.Euler(0, newFacingDirection < 0 ? 0 : 180, 0);
                    lastFacingDirection = newFacingDirection;
                    lastTurnTime = Time.time;
                }
                else
                {
                    // Continue in the same direction
                    velocity.x = lastFacingDirection * speed;
                }
            }
            else
            {
                // No turn, just continue in current direction
                velocity.x = newFacingDirection * speed;
            }

            rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
        }
        else if (!isMoving)
        {
            // Stop horizontal movement when not moving
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    bool IsGrounded()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return false;

        Vector2 rayOrigin = new Vector2(transform.position.x, collider.bounds.min.y + 0.05f);
        float rayLength = 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayerMask);
        bool grounded = hit.collider != null;

        return grounded;
    }

    void Jump()
    {
        if (isJumping) return;

        isJumping = true;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (policeShooter != null)
        {
            // 50% chance of shooting on jump
            if (Random.value < 0.5f)
            {
                policeShooter.shoot();
            }
        }

        StartCoroutine(WaitForLanding());
    }

    IEnumerator WaitForLanding()
    {
        // Wait until we're falling
        yield return new WaitUntil(() => rb.linearVelocity.y < 0);

        // Add a small delay to ensure we're well into the falling phase
        yield return new WaitForSeconds(0.1f);

        // Then wait until we hit the ground
        int frameCounter = 0;
        while (!IsGrounded() && frameCounter < 300) // Safety timeout of ~5 seconds
        {
            frameCounter++;
            yield return null;
        }

        // Reset jump flag only after we've landed
        isJumping = false;
    }

    IEnumerator Movement_Stops_Routine()
    {
        while (true)
        {
            // 10% chance to stop for 3 seconds, checked every 3 seconds
            yield return new WaitForSeconds(check_movement_interval);
            if (Random.Range(0f, 1f) < 0.1f)
            {
                isMoving = false;

                // Pause animation and set to first frame
                animator.speed = 0;
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                animator.Play(stateInfo.fullPathHash, 0, 0.0f);

                yield return new WaitForSeconds(3f);
                isMoving = true;
            }
        }
    }
}