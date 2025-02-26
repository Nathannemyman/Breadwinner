using UnityEngine;
using UnityEngine.InputSystem;

public class PogoStickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float chargeSpeed = 5f;
    [SerializeField] private float maxCharge = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float leanSpeed = 90f; // Degrees per second while holding A/D
    [SerializeField] private float leanReturnSpeed = 45f; // Degrees per second when releasing A/D
    [SerializeField] private float maxLeanAngle = 30f; // Max lean angle in degrees
    [SerializeField] private float compressAmount = .5f;
    [SerializeField] private float compressSpeed = 5f;
    [SerializeField] private float compressReturnSpeed = 10f;
    [SerializeField] private float minLocalY = -0.5f;
    [SerializeField] public float Health = 3;

    [Header("References")]
    [SerializeField] private Transform pivot; // Pivot at player's feet
    [SerializeField] private Transform groundCheck;
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject HobsBody;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public Transform Spawn;
    [SerializeField] private Collider2D BodColl;
    [SerializeField] public bool hasBread;

    [Header("Crash Settings")]
    [SerializeField] private float crashDuration = 1f;
    [SerializeField] private float respawnHeight = 2f;
    [SerializeField] private float crashVelocityThreshold = -5f;
    [SerializeField] private ParticleSystem crashParticles;

    private bool isCrashing = false;
    private float crashTimer;
    private Vector2 crashPosition;

    private float charge = 0f;
    private bool isCharging = false;
    private float currentLeanAngle = 0f; // Current lean angle (positive = right, negative = left)
    private Rigidbody2D rb;
    private PlayerController playerControls;
    private bool wasGrounded; // Track previous frame's grounded state
    [SerializeField] private Vector3 initialLocalPos;

    // Track lean input states
    private bool isLeaningLeft = false;
    private bool isLeaningRight = false;

    public Sprite spriteUp;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    private SpriteRenderer sr;

    void Awake()
    {
        initialLocalPos = body.transform.localPosition;
        rb = GetComponent<Rigidbody2D>();
        sr = body.GetComponent<SpriteRenderer>();
        BodColl = body.GetComponent<Collider2D>();
        playerControls = new PlayerController();
    }

    void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.LeanLeft.started += _ => isLeaningLeft = true;
        playerControls.Player.LeanLeft.canceled += _ => isLeaningLeft = false;
        playerControls.Player.LeanRight.started += _ => isLeaningRight = true;
        playerControls.Player.LeanRight.canceled += _ => isLeaningRight = false;
        playerControls.Player.Charge.started += OnChargeStart;
        playerControls.Player.Charge.canceled += OnChargeRelease;
    }

    void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.LeanLeft.started -= _ => isLeaningLeft = true;
        playerControls.Player.LeanLeft.canceled -= _ => isLeaningLeft = false;
        playerControls.Player.LeanRight.started -= _ => isLeaningRight = true;
        playerControls.Player.LeanRight.canceled -= _ => isLeaningRight = false;
        playerControls.Player.Charge.started -= OnChargeStart;
        playerControls.Player.Charge.canceled -= OnChargeRelease;
    }

    void Update()
    {
        HandleLeaning();
        HandleBread();
        HandleCharge();
        HandleCrash();
        UpdatePivotPosition();
        ResetMomentumOnLanding();
    }

    // --- Input Handlers ---
    void OnChargeStart(InputAction.CallbackContext context)
    {


        if (IsGrounded())
        {
            isCharging = true;
        }
    }

    void OnChargeRelease(InputAction.CallbackContext context)
    {

        if (isCharging)
        {
            isCharging = false;
            Jump();
        }
    }

    // --- Leaning Logic ---
    void HandleLeaning()
    {
        // Determine lean direction
        float leanInput = 0f;
        if (isLeaningLeft && isLeaningRight)
        {
            // Both pressed: stop leaning (freeze angle)
            return;
        }
        else if (isLeaningLeft)
        {
            sr.sprite = spriteLeft;
            leanInput = -1f;
        }
        else if (isLeaningRight)
        {
            sr.sprite = spriteRight;
            leanInput = 1f;
        }

        if (leanInput != 0)
        {
            // Accumulate lean angle
            currentLeanAngle += leanInput * leanSpeed * Time.deltaTime;
            currentLeanAngle = Mathf.Clamp(currentLeanAngle, -maxLeanAngle, maxLeanAngle);
        }
        else
        {
            // Return to upright only if neither key is pressed
            if (!isLeaningLeft && !isLeaningRight && !isCrashing)
            {
                sr.sprite = spriteUp;
                currentLeanAngle = Mathf.MoveTowards(currentLeanAngle, 0f, leanReturnSpeed * Time.deltaTime);
            }
        }

        // Apply rotation to the pivot and player
        RotatePlayerAroundPivot();
    }

    void RotatePlayerAroundPivot()
    {
        // Calculate the offset between the player and the pivot
        Vector2 offset = transform.position - pivot.position;

        // Rotate the offset vector by the current lean angle
        Quaternion rotation = Quaternion.Euler(0, 0, -currentLeanAngle);
        //Vector2 rotatedOffset = rotation * offset;

        // Update the player's position and rotation
        //transform.position = pivot.position + (Vector3)rotatedOffset;
        transform.rotation = rotation;
    }

    // --- Charging & Jumping ---
    void HandleCharge()
    {
        if (isCharging)
        {
            // Move the body downward (local Y axis) during charge
            float newY = Mathf.MoveTowards(
                body.transform.localPosition.y,
                initialLocalPos.y + minLocalY, // Target compressed position
                compressSpeed * Time.deltaTime
            );

            body.transform.localPosition = new Vector3(
                initialLocalPos.x,
                newY,
                initialLocalPos.z
            );

            charge = Mathf.Min(charge + chargeSpeed * Time.deltaTime, maxCharge);
            //body.transform.position = MoveTowardsVector3(body.transform.position, targetCompress, compressSpeed * Time.deltaTime);
        }
        else
        {
            // Return to the original local position
            body.transform.localPosition = Vector3.MoveTowards(
                body.transform.localPosition,
                initialLocalPos,
                compressReturnSpeed * Time.deltaTime
            );
        }
    }

    void Jump()
    {
        //body.transform.position = MoveTowardsVector3(body.transform.position, origionalBody.position, compressReturnSpeed * Time.deltaTime);


        if (!IsGrounded()) return;

        // Calculate jump direction based on lean angle
        float leanDirection = currentLeanAngle / maxLeanAngle; // Normalize to [-1, 1]
        Vector2 jumpDirection = (Vector2)(pivot.up + pivot.right * leanDirection).normalized;

        rb.AddForce(jumpDirection * charge * jumpForce, ForceMode2D.Impulse);
        charge = 0f;
    }

    // --- Ground Handling ---
    void ResetMomentumOnLanding()
    {
        bool isGrounded = IsGrounded();

        // Reset velocity when landing
        if (isGrounded && !wasGrounded)
            rb.linearVelocity = Vector2.zero;

        wasGrounded = isGrounded;

        if (!isCrashing && IsGrounded() && !wasGrounded)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void UpdatePivotPosition()
    {
        // Keep pivot at player's feet
        Vector2 feetPosition = (Vector2)transform.position;
        pivot.position = feetPosition;
        groundCheck.position = feetPosition;
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }


    void HandleCrash()
    {
        // Only check collisions using body's collider
        if (!isCrashing && BodColl.IsTouchingLayers(groundLayer))
        {
            StartCrash();
        }

        if (isCrashing)
        {
            crashTimer += Time.deltaTime;

            if (crashTimer >= crashDuration)
            {
                EndCrash();
            }
        }

        wasGrounded = IsGrounded();
    }

    public void StartCrash()
    {
        isCrashing = true;
        Health--;
        crashTimer = 0f;
        crashPosition = transform.position;

        // Freeze all movement
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        playerControls.Disable();

        // Visual feedback
        if (crashParticles != null)
        {
            crashParticles.transform.position = crashPosition;
            crashParticles.Play();
        }
        sr.color = new Color(1, 0.5f, 0.5f); // Orange tint
    }

    public void EndCrash()
    {
        isCrashing = false;

        // Respawn above crash position
        Vector2 respawnPos = crashPosition + Vector2.up * respawnHeight;
        transform.position = respawnPos;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        currentLeanAngle = 0;

        // Restore physics
        rb.constraints = RigidbodyConstraints2D.None;
        rb.linearVelocity = Vector2.zero;

        // Reset visuals
        sr.color = Color.white;
        if (crashParticles != null) crashParticles.Stop();

        // Re-enable controls
        playerControls.Enable();
    }

    void HandleBread()
    {
        if (hasBread)
        {
            sr.flipX = true;
            HobsBody.GetComponent<SpriteRenderer>().flipX = true;

        }
        else if (!hasBread)
        {
            sr.flipX = false;
            HobsBody.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bread"))
        {
            hasBread = true;
            Destroy(collision.gameObject);
        }
    }
}
