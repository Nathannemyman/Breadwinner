using System.Collections;
using UnityEngine;

public class Civilian_Movement : MonoBehaviour
{
    private float speed;
    private bool isMoving = true;
    private const float check_movement_interval = 3f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        speed = Random.Range(1f, 5f);
        StartCoroutine(Movement_Stops_Routine());
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            animator.speed = 1; // resume animation
        }
    }
    IEnumerator Movement_Stops_Routine()
    {
        while (true)
        {
            // 40% chance to stop for 3 seconds, checked every 3 seconds
            yield return new WaitForSeconds(check_movement_interval);
            if (Random.Range(0f, 1f) < 0.4f)
            {
                isMoving = false;
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                // Pause animation
                animator.speed = 0;
                // Play first frame
                animator.Play(stateInfo.fullPathHash, 0, 0.0f);
                yield return new WaitForSeconds(3f);
                isMoving = true;
            }
        }
    }
}
