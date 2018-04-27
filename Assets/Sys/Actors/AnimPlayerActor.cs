using UnityEngine;

[EActor("动画播放")]
public class AnimPlayerActor : EffectActor
{
    [SerializeField]
    private Animator animator;

    protected override void OnRun()
    {
        if (animator == null)
        {
            return;
        }
        animator.gameObject.SetActive(true);
        animator.enabled = true;
    }

    protected override void OnClose()
    {
        if (animator == null)
        {
            return;
        }
        animator.gameObject.SetActive(false);
        animator.enabled = false;
    }
}