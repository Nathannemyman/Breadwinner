using UnityEngine;
using UnityEngine.Events;

public class AnimationState : StateMachineBehaviour
{
    [SerializeField] private UnityEvent onStateEnter;
    [SerializeField] private UnityEvent onStateExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateEnter?.Invoke();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateExit?.Invoke();
    }
}
