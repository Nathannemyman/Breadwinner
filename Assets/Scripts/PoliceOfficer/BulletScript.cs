using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force;
    public float lifetime = 10f; // How long the bullet exists before despawning

    private static float lastCrashTime = -5f; // Allows first crash immedietly cus -5+5 = 0
    private static float crashCooldown = 5f; // 5 second cooldown after crashing so you don't get stuck in a crashing loop

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        // 2 unit offset so it faces the player center
        direction.y += 2f;

        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);

        // Despawn timer
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (Time.time >= lastCrashTime + crashCooldown)
            {
                PogoStickMovement pogoScript = player.GetComponent<PogoStickMovement>();
                if (pogoScript != null)
                {
                    pogoScript.StartCrash();
                    lastCrashTime = Time.time;
                }
            }
        }
    }
}