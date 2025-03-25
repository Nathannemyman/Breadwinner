using System.Collections;
using UnityEngine;

public class PoliceShooter : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;

    private GameObject player;
    private float timer;
    private float nextShootTime;
    private Transform policeArm;
    private Transform pivot;

    void Start()
    {
        // Shoot randomly every 10-15 seconds
        nextShootTime = Random.Range(10f, 15f);
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

        if (timer > nextShootTime)
        {
            timer = 0;
            nextShootTime = Random.Range(3f, 7f);
            shoot();
        }
    }

    public void shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }
}