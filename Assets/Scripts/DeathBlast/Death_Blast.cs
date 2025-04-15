using UnityEngine;

public class PlayAnimationThenDespawn : MonoBehaviour
{
    [SerializeField] private string animationName = "Blast";
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private bool animationStarted = false;

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Play the animation
            animator.Play(animationName, 0, 0f);
            animationStarted = true;
        }
    }

    void Update()
    {
        if (animationStarted && animator != null)
        {
            // Get current animation state info
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Check if we're playing our target animation and it has finished
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f)
            {
                // Destroy the GameObject
                Destroy(gameObject);
            }
        }
    }
}