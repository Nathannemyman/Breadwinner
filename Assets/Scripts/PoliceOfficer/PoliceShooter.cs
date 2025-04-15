using System.Collections;
using UnityEngine;

public class PoliceShooter : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float minimumShootDistance = 10f;

    private GameObject player;
    private float timer;
    private float nextShootTime;
    private Transform policeArm;
    private Transform pivot;

    void Start()
    {
        // Shoot randomly every 15-25 seconds
        nextShootTime = Random.Range(15f, 25f);
        player = GameObject.FindGameObjectWithTag("Player");

        policeArm = transform.Find("PoliceArm");

        pivot = policeArm.Find("Pivot");

    }

    void Update()
    {
        timer += Time.deltaTime;

        // Rotate the arm to face the player
        if (player != null && policeArm != null && pivot != null)
        {
            // Get the direction from pivot to player with a +2 Y offset (so it faces the center of the player)
            Vector3 playerPosWithOffset = player.transform.position + new Vector3(0, 2, 0);
            Vector3 direction = playerPosWithOffset - pivot.position;

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // flip direction so gun faces player
            angle += 180f;

            // Apply rotation to the PoliceArm around the Pivot
            policeArm.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (timer > nextShootTime && player != null)
        {
            // Calculate distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Only shoot if player is beyond minimum distance
            if (distanceToPlayer > minimumShootDistance)
            {
                timer = 0;
                nextShootTime = Random.Range(3f, 7f);
                shoot();
            }
            else
            {
                // Reset timer but don't shoot
                timer = 0;
                nextShootTime = Random.Range(3f, 7f);
            }
        }
    }

    public void shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }
}