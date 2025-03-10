using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DeathCollision : MonoBehaviour
{
    public float shootSpeed = 30f; // Speed civilian is shot off screen
    public float rotationForce = 20f; // Speed civilian rotates off screen
    public bool invertDirection = false;

    private PogoStickMovement pogoStickMovement;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D parentRb;
    private Collider2D[] allColliders;
    private bool hasRunDeathFunction = false;
    private bool isCollidingWithPogo = false;
    private PlayerInput playerInput;
    private InputAction chargeAction;
    private Rigidbody2D playerBody;
    private Animator civilianAnimator;
    [SerializeField] private AudioClip killCivilianSFX;
    private AudioSource killCivilianAudio;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentRb = transform.parent.GetComponent<Rigidbody2D>();
        allColliders = transform.parent.GetComponentsInChildren<Collider2D>();

        InitializePhysics();

        // Find the player and PogoStickMovement at runtime (because Civilain is a prefab)
        FindPlayerReferences();

        civilianAnimator = transform.parent.GetComponent<Animator>();
        killCivilianAudio = GetComponent<AudioSource>();
    }

    void FindPlayerReferences()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            pogoStickMovement = playerObj.GetComponent<PogoStickMovement>();
            playerBody = playerObj.GetComponent<Rigidbody2D>();

            if (pogoStickMovement != null)
            {
                playerInput = pogoStickMovement.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    chargeAction = playerInput.actions.FindAction("Charge");
                }
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
    IEnumerator PlayKillSFX()
    {
        killCivilianAudio.clip = killCivilianSFX;
        killCivilianAudio.loop = false; //play the kill civilian sfx once
        killCivilianAudio.Play();
        yield return new WaitForSeconds(killCivilianSFX.length);
        killCivilianAudio.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pogo_Bottom_Hitbox" &&
            collision.GetComponent<BoxCollider2D>() != null)
        {
            isCollidingWithPogo = true;
            StartCoroutine(PlayKillSFX());
            //AudioSource.PlayClipAtPoint(killCivilianSFX, transform.position);
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

        if (civilianAnimator != null)
        {
            civilianAnimator.speed = 0; // pause animation
        }

        GameManager.Instance.Money += 15;
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