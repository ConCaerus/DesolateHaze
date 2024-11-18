using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    [SerializeField] Animator animator;
    private string currentAnimation = "";

    private void Start()
    {
        ChangeAnimation("Idle", 0.1f);
    }

    public void CheckAnimation(Vector2 movement, bool falling, PlayerMovement.pMovementState action)
    {
        if (action == PlayerMovement.pMovementState.RopeClimbing && Mathf.Abs(movement.y) > 0f)
        {
            ChangeAnimation("Rope_Climbing", 0.1f);
            animator.speed = 1f;
        }
        else if (action == PlayerMovement.pMovementState.RopeClimbing && Mathf.Abs(movement.y) == 0f)
        {
            animator.speed = 0;
        }
        else if (action == PlayerMovement.pMovementState.RopeClimbing && Mathf.Abs(movement.y) < 0f)
        {
            animator.speed = -1;
        }
        else if (action == PlayerMovement.pMovementState.Falling)
        {
            ChangeAnimation("Jump", 0.1f);
        }
        else if (!falling && Mathf.Abs(movement.x) > 0f)
        {
            ChangeAnimation("Walk", 0.2f);
        }
        else if (currentAnimation != "Idle")
        {
            ChangeAnimation("Idle", 0.1f);
        }
    }
    private void ChangeAnimation(string animation, float crossfade)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
            //Debug.Log("Animation change!");
        }
    }
}
