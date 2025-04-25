using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DeathCollision : MonoBehaviour
{
    public float shootSpeed = 300f; // Speed civilian is shot off screen
    public float rotationForce = 20f; // Speed civilian rotates off screen
    public bool invertDirection = false;
    public bool isPoliceOfficer = false; // New flag to identify police officers

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
    [SerializeField] private AudioClip flyingCivilianSFX;
    [SerializeField] private AudioClip exitSFX;
    private AudioSource killCivilianAudio;
    private AudioSource shootCivilianAudio;
    private int styleBonus = 1;

    void Start()
    {
        GameManager.Instance.policeSpawning = false; // Initially set police not to spawn

        spriteRenderer = GetComponent<SpriteRenderer>();
        parentRb = transform.parent.GetComponent<Rigidbody2D>();
        allColliders = transform.parent.GetComponentsInChildren<Collider2D>();

        InitializePhysics();

        // Find the player and PogoStickMovement at runtime (because Civilain is a prefab)
        FindPlayerReferences();

        civilianAnimator = transform.parent.GetComponent<Animator>();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            killCivilianAudio = audioSources[0];
            shootCivilianAudio = audioSources[1];
        }
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
            string parentTag = transform.parent.tag;


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

        shootCivilianAudio.clip = flyingCivilianSFX;
        shootCivilianAudio.loop = false; //play the flying civilian sfx once
        shootCivilianAudio.Play();
        yield return new WaitForSeconds(killCivilianSFX.length);
        yield return new WaitForSeconds(killCivilianSFX.length);


        if (!CheckVisibility() && hasRunDeathFunction)
        {
            Debug.Log("Exploding Civilian!!");
            // need to assign exit SFX !!!!!!
            killCivilianAudio.clip = exitSFX;
            killCivilianAudio.loop = false; //play the exit sfx(the explosion) once
            killCivilianAudio.Play();
            yield return new WaitForSeconds(exitSFX.length);
            killCivilianAudio.Stop();
        }
        killCivilianAudio.Stop();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pogo_Bottom_Hitbox" && collision.GetComponent<BoxCollider2D>() != null)
        {
            isCollidingWithPogo = true;
            if(pogoStickMovement.frontFlipped)
            {
                styleBonus = 3;
            }
            pogoStickMovement.frontFlipped = false;
            //StartCoroutine(PlayKillSFX());
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
    private bool CheckVisibility()
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position); //checks if the civilian is on the screen
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    private void RunDeathFunction()
    {
        Debug.Log("RUNNING DEATH FUNC");
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
        StartCoroutine(PlayKillSFX()); // taco bell bong plays when civilian dies, not when pogo collides with player
        // Apply force to civilian
        parentRb.AddForce(force, ForceMode2D.Impulse);
        parentRb.AddTorque(rotationForce * (invertDirection ? -1 : 1), ForceMode2D.Impulse);

        if (playerBody != null)
        {
            Vector2 recoilForce = -force * (1f / 200f); // 1/200th of the force recoiled back

            // Check if player's velocity is already above 10
            float currentVelocityMagnitude = playerBody.linearVelocity.magnitude;

            // Debug log player velocity when shooting upward
            if (force.y > 0)
            {
                Debug.Log("Player velocity: " + currentVelocityMagnitude);
            }

            // Only apply force if velocity is below 20
            if (currentVelocityMagnitude < 20f)
            {
                playerBody.AddForce(recoilForce, ForceMode2D.Impulse);
            }
        }

        if (civilianAnimator != null)
        {
            civilianAnimator.speed = 0; // pause animation
        }
        int moneyToAdd = 10 * styleBonus;
        Debug.Log("GOT " + moneyToAdd);
        if (GameData.Instance != null)
        {
            if (GameData.Instance.HasItem(CollectableType.LethalFaceCard))
            {
                if (!isPoliceOfficer)
                {
                    GameData.Instance.AddMoney(moneyToAdd * 2);
                    GameManager.Instance.Money += moneyToAdd * 2;
                }
            }
            else GameData.Instance.AddMoney(moneyToAdd);
            if (!isPoliceOfficer)
            {
                GameManager.Instance.Money += moneyToAdd;
            }
            if (isPoliceOfficer) GameData.Instance.PoliceOfficerKilled();
        }
        else
        {
            if (!isPoliceOfficer)
            {
                GameManager.Instance.Money += moneyToAdd;
            }
        }
        GameManager.Instance.policeSpawning = true;
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
