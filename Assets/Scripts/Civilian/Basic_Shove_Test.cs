using UnityEngine;
using System.Collections;

public class ParentAnimationController : MonoBehaviour
{
    private Animator parentAnimator;
    private bool isCoroutineRunning = false;

    void Start()
    {
        // Get the Animator component from the parent
        parentAnimator = transform.parent.GetComponent<Animator>();

        // Check if we found the Animator
        if (parentAnimator != null)
        {
            // Start the punching cycle coroutine
            StartCoroutine(PunchingCycle());
        }
        else
        {
            Debug.LogWarning("No Animator component found on parent object!");
        }
    }

    IEnumerator PunchingCycle()
    {
        isCoroutineRunning = true;

        while (isCoroutineRunning)
        {
            // Set punching to true
            parentAnimator.SetBool("IsPunching", true);

            // Wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);

            // Set punching to false
            parentAnimator.SetBool("IsPunching", false);

            // Wait for 5 seconds before repeating
            yield return new WaitForSeconds(5.0f);

            // Total cycle time: 0.1s + 5.0s = 5.1s
        }
    }

    void OnDisable()
    {
        // Stop the coroutine when the object is disabled
        isCoroutineRunning = false;
        StopAllCoroutines();
    }
}