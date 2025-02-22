using UnityEngine;
using UnityEngine.InputSystem;

public class DeathCollision : MonoBehaviour
{
    public PogoStickMovement pogoStickMovement;
    public float shootSpeed = 30f; // Speed civilian is shot off screen
    public float rotationForce = 20f; // Speed civilian rotates off screen
    public bool invertDirection = false;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D parentRb;
    private Collider2D[] allColliders;
    private bool hasRunDeathFunction = false;
    private bool isCollidingWithPogo = false;
    private PlayerInput playerInput;
    private InputAction chargeAction;
    private Rigidbody2D playerBody;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentRb = transform.parent.GetComponent<Rigidbody2D>();
        allColliders = transform.parent.GetComponentsInChildren<Collider2D>();

        InitializePhysics();

        Transform playerTransform = transform.root.Find("Player");
        if (playerTransform != null)
        {
            Transform bodyTransform = playerTransform.Find("Body");
            if (bodyTransform != null)
            {
                playerBody = bodyTransform.GetComponent<Rigidbody2D>();
            }
        }

        if (pogoStickMovement != null)
        {
            playerInput = pogoStickMovement.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                chargeAction = playerInput.actions.FindAction("Charge");
            }
        }
    }


    void InitializePhysics()
    {
        if (parentRb != null)
        {
            parentRb.bodyType = RigidbodyType2D.Kinematic;
            parentRb.constraints = RigidbodyConstraints2D.FreezeRotation;

            foreach (Collider2D col in allColliders)
            {
                col.enabled = true;
            }
        }
    }

    void Update()
    {
        if (!hasRunDeathFunction &&
            isCollidingWithPogo &&
            chargeAction != null &&
            chargeAction.IsPressed())
        {
            RunDeathFunction();
            hasRunDeathFunction = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pogo_Bottom_Hitbox" &&
            collision.GetComponent<BoxCollider2D>() != null)
        {
            isCollidingWithPogo = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pogo_Bottom_Hitbox" &&
            collision.GetComponent<BoxCollider2D>() != null)
        {
            isCollidingWithPogo = false;
        }
    }

    private void RunDeathFunction()
    {
        spriteRenderer.color = Color.white;

        if (parentRb == null) return;

        Collider2D[] currentColliders = transform.parent.GetComponentsInChildren<Collider2D>();

        parentRb.bodyType = RigidbodyType2D.Dynamic;
        parentRb.constraints = RigidbodyConstraints2D.None;

        foreach (Collider2D col in currentColliders)
        {
            col.enabled = false;
        }

        Vector2 shootDirection = GetShootDirection();
        Vector2 force = shootDirection.normalized * shootSpeed;

        // Apply force to civilian
        parentRb.AddForce(force, ForceMode2D.Impulse);
        parentRb.AddTorque(rotationForce * (invertDirection ? -1 : 1), ForceMode2D.Impulse);

        if (playerBody != null)
        {
            Vector2 recoilForce = -force * (1f / 4f); // 1/4th of the force recoiled back
            playerBody.AddForce(recoilForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 GetShootDirection()
    {
        if (pogoStickMovement.player_rotation != null)
        {
            Vector3 baseDirection = Vector3.down;
            Vector3 rotatedDirection = pogoStickMovement.player_rotation * baseDirection;

            Vector2 direction = new Vector2(rotatedDirection.x, rotatedDirection.y);

            float angle = pogoStickMovement.player_rotation.eulerAngles.z;
            direction.y += Mathf.Sin(angle * Mathf.Deg2Rad) * 0.5f;

            return invertDirection ? -direction : direction;
        }

        return invertDirection ? Vector2.left : Vector2.right;
    }
}