using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorBoolSetter : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }
    
    public void SetBoolTrue(string boolName)
    {
        anim.SetBool(boolName, true);
    }

    public void SetBoolFalse(string boolName)
    {
        anim.SetBool(boolName, false);
    }
}
