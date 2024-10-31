using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    [SerializeField] Animator animator;
    private string currentAnimation = "";

    private void Start()
    {
        ChangeAnimation("Idle");
    }

    public void CheckAnimation(float movement, bool falling)
    {
        if (!falling && Mathf.Abs(movement) > 0f)
        {
            ChangeAnimation("Walk", 0.2f);
            //Debug.Log("Walking!!");
        }
        else if (currentAnimation != "Idle")
        {
            ChangeAnimation("Idle", 0.2f);
            //Debug.Log("Idling!!");
        }
    }
    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
            //Debug.Log("Animation change!");
        }
    }
}
