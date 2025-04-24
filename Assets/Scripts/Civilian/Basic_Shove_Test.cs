using UnityEngine;

public class ParentAnimationController : MonoBehaviour
{
    private Animator parentAnimator;

    void Start()
    {
        // Get the Animator component from the parent
        parentAnimator = transform.parent.GetComponent<Animator>();

        // Check if we found the Animator
        if (parentAnimator != null)
        {
            // Set the "IsPunching" parameter to true
            parentAnimator.SetBool("IsPunching", true);
        }
        else
        {
            Debug.LogWarning("No Animator component found on parent object!");
        }
    }
}