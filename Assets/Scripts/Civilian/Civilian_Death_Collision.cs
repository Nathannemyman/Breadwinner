using UnityEngine;
using UnityEngine.InputSystem;

public class DeathCollision : MonoBehaviour
{
    public PogoStickMovement pogoStickMovement;
    public float shootSpeed = 50f;
    public float rotationForce = 200f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D parentRb;
    private Collider2D[] allColliders;
    private bool hasRunDeathFunction = false;
    private bool isCollidingWithPogo = false;
    private PlayerInput playerInput;
    private InputAction chargeAction;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentRb = transform.parent.GetComponent<Rigidbody2D>();
        allColliders = transform.parent.GetComponentsInChildren<Collider2D>();

        InitializePhysics();

        if (pogoStickMovement != null)
        {
            playerInput = pogoStickMovement.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                chargeAction = playerInput.actions.FindAction("Charge");
                if (chargeAction == null)
                {
                    Debug.LogError("Charge action not found in PlayerInput's actions.");
                }
            }
            else
            {
                Debug.LogError("PlayerInput component not found on PogoStickMovement's GameObject.");
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
        spriteRenderer.color = Color.black;

        if (parentRb == null) return;

        parentRb.bodyType = RigidbodyType2D.Dynamic;
        parentRb.constraints = RigidbodyConstraints2D.None;

        foreach (Collider2D col in allColliders)
        {
            col.enabled = false;
        }

        float rotation = pogoStickMovement.player_rotation.eulerAngles.z;
        Vector2 shootDirection = Quaternion.Euler(0, 0, rotation) * Vector2.right;

        parentRb.AddForce(shootDirection.normalized * shootSpeed, ForceMode2D.Impulse);
        parentRb.AddTorque(rotationForce, ForceMode2D.Impulse);
    }
}