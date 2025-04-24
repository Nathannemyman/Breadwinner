using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PogoStickMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float leanTorque = 50f;
    [SerializeField] public float gravityTorque = 10f;
    [SerializeField] public float uprightThreshold = 10f;
    [SerializeField] public float gravityStrength = 50f;
    [SerializeField] public float chargeSpeed = 5f;
    [SerializeField] private float maxCharge = 10f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float leanSpeed = 90f; // Degrees per second while holding A/D
    [SerializeField] private float inAirLeanMult = 2;
    private float tempLeanSpeed;
    [SerializeField] private float leanReturnSpeed = 45f; // Degrees per second when releasing A/D
    //[SerializeField] private float maxLeanAngle = 30f; // Max lean angle in degrees
    [SerializeField] private float compressSpeed = 5f;
    [SerializeField] private float compressReturnSpeed = 10f;
    [SerializeField] private float minLocalY = -0.5f;

    [Header("Leaning Animation Settings")]
    [SerializeField] private float leanTransitionTime = 0.1f; // Total time for leaning transition
    [SerializeField] private Sprite leftInbetween1; // First inbetween frame for leaning left
    [SerializeField] private Sprite leftInbetween2; // Second inbetween frame for leaning left
    [SerializeField] private Sprite rightInbetween1; // First inbetween frame for leaning right
    [SerializeField] private Sprite rightInbetween2; // Second inbetween frame for leaning right

    [Header("References")]
    [SerializeField] private Transform pivot; // Pivot at player's feet
    [SerializeField] private Transform groundCheck;
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject HobsBody;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public Transform Spawn;
    [SerializeField] private Collider2D BodColl;
    [SerializeField] public bool HasBread;
    [SerializeField] private AudioClip chargeReleaseSFX;
    [SerializeField] private AudioClip chargeStartSFX;
    [SerializeField] private AudioClip chargeHoldSFX;
    private AudioSource audioSource;

    [Header("Crash Settings")]
    [SerializeField] private float crashDuration = 1f;
    [SerializeField] private float respawnHeight = 2f;
    [SerializeField] private ParticleSystem crashParticles;
    [SerializeField] private float invincibilityBounceForce = 10f; // Force to apply when hitting ground while invincible


    [Header("Bounce Settings")]
    [SerializeField] private float bounceWindowDuration = 0.1f;
    [SerializeField] private float bounceFactor = 0.7f;       
    private float bounceWindowTimer = 0f;           
    private float lastJumpCharge = 0f;

    private bool isCrashing = false;
    private float crashTimer;
    private Vector2 crashPosition;
    private bool flipped = false; //checks if sprite is flipped after getting bread

    private float charge = 0f;
    private bool isCharging = false;
    [SerializeField] private float currentLeanAngle = 0f; // Current lean angle (positive = right, negative = left)
    private Rigidbody2D rb;
    private PlayerController playerControls;
    [SerializeField] private bool wasGrounded = false; // Track previous frame's grounded state
    [SerializeField] private Vector3 initialLocalPos;

    // Track lean input states
    private bool isLeaningLeft = false;
    private bool isLeaningRight = false;
    private bool isTransitioning = false;
    private Coroutine leanCoroutine;

    public Sprite spriteUp;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    public Sprite stunnedSprite;
    private SpriteRenderer sr;
    private float BodCollOffset;
    private Vector2 currentOffset;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private float invincibilityDuration = 1f; // 1 second of invincibility after crashing out

    public bool stunned = false;

    public bool canDoubleJump;
    public bool doubleJump = true;
    public bool frontFlipped = false;
    public float energyValue;

    //Added code from Adam
    private static PogoStickMovement _instance;
    public static PogoStickMovement Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }
    public Quaternion player_rotation // Meant to reference the player rotation for external use
    {
        get { return transform.rotation; }
    }
    // Crashout sprite stuff
    [SerializeField] private GameObject pogoStickHitbox;
    [SerializeField] private GameObject crashOutFront;
    [SerializeField] private GameObject crashOutBack;
    [SerializeField] private GameObject bodyHitbox;

    void Awake()
    {
        initialLocalPos = body.transform.localPosition;
        rb = GetComponent<Rigidbody2D>();
        sr = HobsBody.GetComponent<SpriteRenderer>();
        BodColl = HobsBody.GetComponent<Collider2D>();
        playerControls = new PlayerController();
        BodCollOffset = BodColl.offset.x;
        currentOffset = BodColl.offset;
        rb.centerOfMass = pivot.localPosition;
        audioSource = GetComponent<AudioSource>();
        tempLeanSpeed = leanSpeed;
    }

    void Start()
    {
        GameManager.Instance.HasBread = false;

        if (GameData.Instance != null)
        {
            if (GameData.Instance.HasItem(CollectableType.EnergyDrink))
            {
                energyValue = energyValue * 1.5f;
                GetComponent<Rigidbody2D>().gravityScale = GetComponent<Rigidbody2D>().gravityScale * 1.5f;
                Debug.Log("ENERGY DRINK IN HAND");
            }

            if (GameData.Instance.HasItem(CollectableType.GoopiterBatteryCharge))
            {
                chargeSpeed = chargeSpeed * 2;
                Debug.Log("BATTERY IN HAND");
            }

            if (GameData.Instance.HasItem(CollectableType.ZaarianRocketBooster))
            {
                canDoubleJump = true;
                Debug.Log("ROCKET BOOTS IN HAND");
            }
        }

        crashOutBack.SetActive(false);
        crashOutFront.SetActive(false);
    }

    void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.LeanLeft.started += OnLeanLeftStart;
        playerControls.Player.LeanLeft.canceled += OnLeanLeftEnd;
        playerControls.Player.LeanRight.started += OnLeanRightStart;
        playerControls.Player.LeanRight.canceled += OnLeanRightEnd;
        playerControls.Player.Charge.started += OnChargeStart;
        playerControls.Player.Charge.canceled += OnChargeRelease;
    }

    void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.LeanLeft.started -= OnLeanLeftStart;
        playerControls.Player.LeanLeft.canceled -= OnLeanLeftEnd;
        playerControls.Player.LeanRight.started -= OnLeanRightStart;
        playerControls.Player.LeanRight.canceled -= OnLeanRightEnd;
        playerControls.Player.Charge.started -= OnChargeStart;
        playerControls.Player.Charge.canceled -= OnChargeRelease;
    }

    // Improved coroutine for transitioning to leaning
    IEnumerator TransitionToLeaning(bool isRight)
    {
        isTransitioning = true;
        float frameTime = leanTransitionTime / 2f;

        // Determine which sprites to use based on direction and whether flipped
        Sprite inbetween1, inbetween2, finalSprite;
        if (isRight)
        {
            if (!flipped)
            {
                inbetween1 = rightInbetween1;
                inbetween2 = rightInbetween2;
                finalSprite = spriteRight;
            }
            else
            {
                inbetween1 = leftInbetween1;
                inbetween2 = leftInbetween2;
                finalSprite = spriteLeft;
            }
        }
        else
        {
            if (!flipped)
            {
                inbetween1 = leftInbetween1;
                inbetween2 = leftInbetween2;
                finalSprite = spriteLeft;
            }
            else
            {
                inbetween1 = rightInbetween1;
                inbetween2 = rightInbetween2;
                finalSprite = spriteRight;
            }
        }

        // First inbetween frame
        sr.sprite = inbetween1;
        yield return new WaitForSeconds(frameTime);

        // Check if direction changed during the frame delay
        if ((isRight && !isLeaningRight) || (!isRight && !isLeaningLeft))
        {
            isTransitioning = false;
            yield break;
        }

        // Second inbetween frame
        sr.sprite = inbetween2;
        yield return new WaitForSeconds(frameTime);

        // Check if direction changed during the frame delay
        if ((isRight && !isLeaningRight) || (!isRight && !isLeaningLeft))
        {
            isTransitioning = false;
            yield break;
        }

        // Final frame
        sr.sprite = finalSprite;
        isTransitioning = false;
    }

    // Improved coroutine for transitioning back to upright
    IEnumerator TransitionToUpright(bool fromRight)
    {
        isTransitioning = true;
        float frameTime = leanTransitionTime / 2f;

        // Determine which sprites to use based on direction and whether flipped
        Sprite inbetween2, inbetween1;
        if (fromRight)
        {
            if (!flipped)
            {
                inbetween2 = rightInbetween1;
                inbetween1 = rightInbetween2;
            }
            else
            {
                inbetween2 = leftInbetween1;
                inbetween1 = leftInbetween2;
            }
        }
        else
        {
            if (!flipped)
            {
                inbetween2 = leftInbetween1;
                inbetween1 = leftInbetween2;
            }
            else
            {
                inbetween2 = rightInbetween1;
                inbetween1 = rightInbetween2;
            }
        }

        // First transition frame
        sr.sprite = inbetween1;
        yield return new WaitForSeconds(frameTime);

        // Check if direction changed during the frame delay
        if (isLeaningLeft || isLeaningRight)
        {
            isTransitioning = false;
            yield break;
        }

        // Second transition frame
        sr.sprite = inbetween2;
        yield return new WaitForSeconds(frameTime);

        // Check if direction changed during the frame delay
        if (isLeaningLeft || isLeaningRight)
        {
            isTransitioning = false;
            yield break;
        }

        // Final upright frame
        sr.sprite = spriteUp;
        isTransitioning = false;
    }

    // Also modify your input handlers to correctly handle interruptions
    void OnLeanLeftStart(InputAction.CallbackContext context)
    {
        isLeaningLeft = true;

        if (!isCrashing)
        {
            if (leanCoroutine != null)
            {
                StopCoroutine(leanCoroutine);
                isTransitioning = false;
            }
            leanCoroutine = StartCoroutine(TransitionToLeaning(false));
        }
    }

    void OnLeanRightStart(InputAction.CallbackContext context)
    {
        isLeaningRight = true;

        if (!isCrashing)
        {
            if (leanCoroutine != null)
            {
                StopCoroutine(leanCoroutine);
                isTransitioning = false;
            }
            leanCoroutine = StartCoroutine(TransitionToLeaning(true));
        }
    }

    void OnLeanLeftEnd(InputAction.CallbackContext context)
    {
        isLeaningLeft = false;

        if (!isLeaningRight && !isCrashing)
        {
            if (leanCoroutine != null)
            {
                StopCoroutine(leanCoroutine);
                isTransitioning = false;
            }
            leanCoroutine = StartCoroutine(TransitionToUpright(false));
        }
    }

    void OnLeanRightEnd(InputAction.CallbackContext context)
    {
        isLeaningRight = false;

        if (!isLeaningLeft && !isCrashing)
        {
            if (leanCoroutine != null)
            {
                StopCoroutine(leanCoroutine);
                isTransitioning = false;
            }
            leanCoroutine = StartCoroutine(TransitionToUpright(true));
        }
    }

    // Offsets the player rotation so that they're off-balance when shoved by civilian (for external use)
    public void ApplyExternalRotation(float rotationAmount)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotationAmount);
        currentLeanAngle = rotationAmount;
        //currentLeanAngle = Mathf.Clamp(currentLeanAngle, -maxLeanAngle, maxLeanAngle);
    }


    void FixedUpdate()
    {
        // Decrement bounce window timer if active
        if (bounceWindowTimer > 0)
        {
            bounceWindowTimer -= Time.fixedDeltaTime;
        }
        HandleLeaning();
        HandleBread();
        HandleCharge();
        ResetMomentumOnLanding();
        HandleCrash();
        HandleStunnedState();
    }

    IEnumerator ChargingSFX()
    {
        audioSource.clip = chargeStartSFX;
        audioSource.loop = false; //play the starting sfx once
        audioSource.Play();
        yield return new WaitForSeconds(chargeStartSFX.length);
        if (isCharging)
        {
            audioSource.clip = chargeHoldSFX;
            audioSource.loop = true; //loop the charge hold sound effect
            audioSource.Play();
        }
        //audioSource.clip = chargeHoldSFX;
        //audioSource.loop = true; //loop the charge hold sound effect
        //audioSource.Play();
    }
    // --- Input Handlers ---
    void OnChargeStart(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            StartCoroutine(ChargingSFX());
        }
        isCharging = true;

    }

    void OnChargeRelease(InputAction.CallbackContext context)
    {
        rb.constraints = RigidbodyConstraints2D.None;

        if (IsGrounded() && bounceWindowTimer > 0)
        {
            // Perform the bounce using a fraction of the last jump's charge
            BounceJump(lastJumpCharge * bounceFactor);

            bounceWindowTimer = 0f;
            isCharging = false;
            charge = 0f;

            audioSource.Stop();
            audioSource.clip = chargeReleaseSFX;
            audioSource.loop = false;
            audioSource.Play();

            return;
        }

        if (isCharging)
        {
            isCharging = false;

            audioSource.Stop();
            audioSource.clip = chargeReleaseSFX;
            audioSource.loop = false;
            audioSource.Play();

            Jump();

            if (canDoubleJump && doubleJump && !wasGrounded)
            {
                Jump();
                Debug.Log("Double Jump");
                doubleJump = false;
            }
        }
    }

    // --- Leaning Logic ---
    void HandleLeaning()
    {
        if (!IsGrounded())
        {
            leanSpeed = inAirLeanMult * tempLeanSpeed;
        }
        else
        {
            leanSpeed = tempLeanSpeed;
        }

        // Determine lean direction
        float leanInput = 0f;
        if (isLeaningLeft && isLeaningRight)
        {
            // Both pressed: stop leaning (freeze angle)
            return;
        }
        else if (isLeaningLeft)
        {
            leanInput = -1f;
        }
        else if (isLeaningRight)
        {
            leanInput = 1f;
        }

        if (leanInput != 0)
        {
            // Accumulate lean angle (input-driven)
            currentLeanAngle += leanInput * leanSpeed * Time.deltaTime;
        }
        else if (!isLeaningLeft && !isLeaningRight && !isCrashing)
        {
            // Don't change the sprite here if we're transitioning
            if (!isTransitioning)
            {
                sr.sprite = spriteUp;
            }

            // Apply gravity or return to upright
            if (Mathf.Abs(currentLeanAngle) > uprightThreshold)
            {
                // Apply gravity-based angular acceleration
                float angleRad = currentLeanAngle * Mathf.Deg2Rad;
                float gravityTorque = Mathf.Sin(angleRad) * gravityStrength;
                currentLeanAngle += gravityTorque * Time.deltaTime;
            }
            else
            {
                // Return to upright position
                currentLeanAngle = Mathf.MoveTowards(currentLeanAngle, 0f, leanReturnSpeed * Time.deltaTime);
            }
        }
        if (currentLeanAngle >= 360 || currentLeanAngle <= -360)
        {
            Debug.Log("READY!");
            frontFlipped = true;
            currentLeanAngle = 0;
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

            // Only increase charge when grounded
            if (IsGrounded() || !IsGrounded() && canDoubleJump && doubleJump)
            {
                charge = Mathf.Min(charge + chargeSpeed * Time.deltaTime, maxCharge);
            }
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


        if (!IsGrounded() && !doubleJump)
        {
            return;
        }


        rb.constraints = RigidbodyConstraints2D.None;

        // Calculate jump direction based on lean angle
        float leanDirection = (transform.rotation.z % 360f) / 180f;
        Vector2 jumpDirection = (Vector2)(pivot.up + pivot.right * leanDirection).normalized;

        rb.AddForce(jumpDirection * charge * jumpForce, ForceMode2D.Impulse);

        // Store the charge used for this jump for later
        lastJumpCharge = charge;

        charge = 0f; // Reset charge after jump
    }

    void BounceJump(float bounceChargeUsed)
    {
        rb.constraints = RigidbodyConstraints2D.None;

        // Calculate jump direction (same logic as regular jump)
        float leanDirection = (transform.rotation.z % 360f) / 180f;
        Vector2 jumpDirection = (Vector2)(pivot.up + pivot.right * leanDirection).normalized;

        // Apply the bounce force (using the stored last jump's charge * factor)
        rb.AddForce(jumpDirection * bounceChargeUsed * jumpForce, ForceMode2D.Impulse);
    }

    void ApplyInvincibilityBounce()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        // Apply an immediate upward force impulse
        rb.AddForce(Vector2.up * invincibilityBounceForce, ForceMode2D.Impulse);

    }

    // --- Ground Handling ---
    void ResetMomentumOnLanding()
    {
        bool currentlyGrounded = IsGrounded(); // Check once per frame

        // Reset velocity and start bounce window when landing
        if (currentlyGrounded && !wasGrounded)
        {
            if(canDoubleJump)
            {
                doubleJump = true;
            }
            frontFlipped = false;
            Vector2 currentVelocity = rb.linearVelocity;
            // 30% speed decrease should make floor less splippery
            rb.linearVelocity = currentVelocity * 0.7f;

            bounceWindowTimer = bounceWindowDuration;
        }
        wasGrounded = currentlyGrounded;
    }



    public bool IsGrounded()
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
        bool touchingGround = BodColl.IsTouchingLayers(groundLayer);

        if (!isCrashing)
        {
            if (touchingGround)
            {
                if (isInvincible)
                {
                    ApplyInvincibilityBounce(); // Push player back up from ground if invincibile to prevent clipping
                }
                else
                {
                    StartCrash();
                }
            }
        }

        if (isCrashing)
        {
            crashTimer += Time.deltaTime;

            if (crashTimer >= crashDuration)
            {
                EndCrash();
            }
        }

        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;

            if (invincibilityTimer >= invincibilityDuration)
            {
                isInvincible = false;
            }
        }

        wasGrounded = IsGrounded();
    }

    void HandleStunnedState()
    {
        // If stunned, override the current sprite
        if (stunned)
        {
            sr.sprite = stunnedSprite;
        }
    }

    public void StartCrash()
    {
        isCrashing = true;
        crashTimer = 0f;
        crashPosition = transform.position;

        GameManager.Instance.DamagePlayer();

        // Store player velocity at time of crash
        Vector2 crashVelocity = rb.linearVelocity;

        // Get position and rotation for crash sprites
        Vector3 crashSpritePosition = (HobsBody != null) ? HobsBody.transform.position : crashPosition;
        Quaternion crashSpriteRotation = (HobsBody != null) ? HobsBody.transform.rotation : transform.rotation;

        // Apply 90-degree rotation offset
        Quaternion rotationOffset = Quaternion.Euler(0, 0, 90);
        Quaternion finalRotation = crashSpriteRotation * rotationOffset;

        // Set position and rotation of crash sprites to match with offset
        if (crashOutFront != null)
        {
            crashOutFront.transform.position = crashSpritePosition;
            crashOutFront.transform.rotation = finalRotation;

            // Apply velocity to front crash sprite
            Rigidbody2D frontRb = crashOutFront.GetComponent<Rigidbody2D>();
            if (frontRb == null)
            {
                // Add Rigidbody2D if it doesn't exist
                frontRb = crashOutFront.AddComponent<Rigidbody2D>();
            }
            frontRb.linearVelocity = crashVelocity;
        }

        if (crashOutBack != null)
        {
            crashOutBack.transform.position = crashSpritePosition;
            crashOutBack.transform.rotation = finalRotation;

            // Apply velocity to back crash sprite
            Rigidbody2D backRb = crashOutBack.GetComponent<Rigidbody2D>();
            if (backRb == null)
            {
                // Add Rigidbody2D if it doesn't exist
                backRb = crashOutBack.AddComponent<Rigidbody2D>();
            }
            backRb.linearVelocity = crashVelocity;
        }

        // Change layer for this object and all children except crash sprites
        ChangeLayerRecursively(gameObject, LayerMask.NameToLayer("Everything_But_Ground"));

        // Make the main sprite invisible
        if (sr != null)
        {
            sr.color = new Color(1, 1, 1, 0);
        }

        // Make Pogo_Bottom_Hitbox transparent
        if (pogoStickHitbox != null)
        {
            SpriteRenderer pogoRenderer = pogoStickHitbox.GetComponent<SpriteRenderer>();
            if (pogoRenderer != null)
            {
                pogoRenderer.color = new Color(1, 1, 1, 0);
            }
        }

        // Handle body transparency with null check and recovery
        if (bodyHitbox == null)
        {
            // Try to find the body reference if it's null after scene reset
            bodyHitbox = GameObject.Find("Body");
        }

        if (bodyHitbox != null)
        {
            SpriteRenderer bodyRenderer = bodyHitbox.GetComponent<SpriteRenderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.color = new Color(1, 1, 1, 0);
            }
        }

        float actualRotation = transform.rotation.eulerAngles.z;
        Debug.Log("Crash Rotation: " + actualRotation);

        // Show appropriate crash sprite based on lean direction
        if (crashOutFront != null && crashOutBack != null)
        {
            if (actualRotation < 180 || actualRotation > 300)
            {
                crashOutFront.SetActive(false);
                crashOutBack.SetActive(true);
            }
            else
            {
                crashOutFront.SetActive(true);
                crashOutBack.SetActive(false);
            }
        }

        // Freeze movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        playerControls.Disable();

        // Visual feedback
        if (crashParticles != null)
        {
            crashParticles.transform.position = crashPosition;
            crashParticles.Play();
        }
    }
    public void EndCrash()
    {
        // Change layer back to Default for this object and all children except crash sprites
        ChangeLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));

        // Hide crash sprites and reset their velocity
        if (crashOutFront != null)
        {
            crashOutFront.SetActive(false);
            Rigidbody2D frontRb = crashOutFront.GetComponent<Rigidbody2D>();
            if (frontRb != null)
            {
                frontRb.linearVelocity = Vector2.zero;
            }
        }

        if (crashOutBack != null)
        {
            crashOutBack.SetActive(false);
            Rigidbody2D backRb = crashOutBack.GetComponent<Rigidbody2D>();
            if (backRb != null)
            {
                backRb.linearVelocity = Vector2.zero;
            }
        }

        // Make main sprite visible again
        if (sr != null) sr.color = Color.white;

        // Make Pogo_Bottom_Hitbox visible again
        if (pogoStickHitbox != null)
        {
            SpriteRenderer pogoRenderer = pogoStickHitbox.GetComponent<SpriteRenderer>();
            if (pogoRenderer != null)
            {
                pogoRenderer.color = new Color(1, 1, 1, 1);
            }
        }

        // Make bodyHitbox visible again
        if (bodyHitbox != null)
        {
            SpriteRenderer bodyRenderer = bodyHitbox.GetComponent<SpriteRenderer>();
            if (bodyRenderer != null)
            {
                bodyRenderer.color = new Color(1, 1, 1, 1);
            }
        }

        isCrashing = false;

        // Start invincibility period
        isInvincible = true;
        invincibilityTimer = 0f;

        // Respawn above crash position
        Vector2 respawnPos = crashPosition + Vector2.up * respawnHeight;
        transform.position = respawnPos;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        currentLeanAngle = 0;

        // Restore physics
        rb.constraints = RigidbodyConstraints2D.None;
        rb.linearVelocity = Vector2.zero;

        // Visual feedback
        if (crashParticles != null) crashParticles.Stop();

        // Re-enable controls
        playerControls.Enable();
    }

    // Helper method to change layers for an object and all its children, excluding crash sprites
    public void ChangeLayerRecursively(GameObject obj, int newLayer)
    {
        // Skip null checks
        if (obj == null) return;

        // Skip the crash sprite objects
        if (obj == crashOutFront || obj == crashOutBack) return;

        // Change the layer of the current object
        obj.layer = newLayer;

        // Change the layer of all children
        foreach (Transform child in obj.transform)
        {
            if (child != null && child.gameObject != crashOutFront && child.gameObject != crashOutBack)
            {
                ChangeLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

    // Find stuff when scene resets
    private void ResetReferences()
    {
        if (bodyHitbox == null)
        {
            bodyHitbox = GameObject.Find("Body");
        }

        if (pogoStickHitbox == null)
        {
            pogoStickHitbox = GameObject.Find("Pogo_Bottom_Hitbox");
        }

        if (crashOutFront == null)
        {
            crashOutFront = GameObject.Find("CrashOutFront");
        }

        if (crashOutBack == null)
        {
            crashOutBack = GameObject.Find("CrashOutBack");
        }
    }

    void HandleBread()
    {
        if (GameManager.Instance.HasBread)
        {
            flipped = true;
            sr.flipX = true;
            currentOffset.x = -BodCollOffset;
            BodColl.offset = currentOffset;
            HobsBody.GetComponent<SpriteRenderer>().flipX = true;

        }
        else if (!GameManager.Instance.HasBread)
        {
            flipped = false;
            sr.flipX = false;
            currentOffset.x = BodCollOffset;
            BodColl.offset = currentOffset;
            HobsBody.GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}
