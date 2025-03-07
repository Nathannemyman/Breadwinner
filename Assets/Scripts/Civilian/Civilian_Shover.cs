using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Civilian_Shover : MonoBehaviour
{
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float upwardForce = 10f;
    [SerializeField] private float timeToActivate = 3f;
    [SerializeField] private float rotationAngle = 30f;

    private Rigidbody2D playerRigidbody;
    private PogoStickMovement pogoMovement;
    private Coroutine pushCoroutine;
    private bool playerInside = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigidbody = other.GetComponent<Rigidbody2D>();
            pogoMovement = other.GetComponent<PogoStickMovement>();

            playerInside = true;

            pushCoroutine = StartCoroutine(PushPlayerAfterDelay());

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Stop the timer if the player exits early
            if (pushCoroutine != null)
            {
                StopCoroutine(pushCoroutine);
                pushCoroutine = null;
            }
        }
    }

    private IEnumerator PushPlayerAfterDelay()
    {
        yield return new WaitForSeconds(timeToActivate);

        // Check if player is still inside and if so apply forces/rotations
        if (playerInside && playerRigidbody != null && pogoMovement != null)
        {
            playerRigidbody.linearVelocity = new Vector2(-pushForce, upwardForce);

            pogoMovement.ApplyExternalRotation(rotationAngle);
        }

        pushCoroutine = null;
    }
}