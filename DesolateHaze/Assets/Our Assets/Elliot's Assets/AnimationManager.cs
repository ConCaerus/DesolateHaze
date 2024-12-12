using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    [SerializeField] Animator animator;
    private string currentAnimation = "";

    public GameObject playerRig;
    Collider[] ragdollColliders;
    Rigidbody[] limbsRigidbodies;

    private void Start()
    {
        ChangeAnimation("Idle", 0.1f);
        RagdollMode(false);
    }

    public void RagdollMode(bool state)
    {
        ragdollColliders = playerRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = playerRig.GetComponentsInChildren<Rigidbody>();

        if(state)
        {
            GetComponent<Animator>().enabled = false;
        }
     
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = state;
        }
        foreach(Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = !state;
        }
    }

    //Checks for player's curState and other factors to identify current action and pass updates
    public void CheckAnimation(Vector2 movement, bool falling, PlayerMovement.pMovementState action)
    {
        //for Push/Pull, check direction player facing when entering state then check direction?
        if (action == PlayerMovement.pMovementState.RopeClimbing && Mathf.Abs(movement.y) > 0f)
        {
            ChangeAnimation("Rope_Climbing", 0.1f);
            animator.speed = 1f;
        }
        else if (action == PlayerMovement.pMovementState.LadderClimbing && Mathf.Abs(movement.y) > 0f)
        {
            ChangeAnimation("Ladder_Climbing", 0.1f);
            animator.speed = 1f;
        }
        else if (action == PlayerMovement.pMovementState.RopeClimbing || action == PlayerMovement.pMovementState.LadderClimbing && Mathf.Abs(movement.y) == 0f)
        {
            animator.speed = 0f;
        }
        else if (action == PlayerMovement.pMovementState.RopeClimbing || action == PlayerMovement.pMovementState.LadderClimbing && Mathf.Abs(movement.y) < 0f)
        {
            animator.speed = -1f;
        }
        else if (action == PlayerMovement.pMovementState.Falling)
        {
            ChangeAnimation("Jump", 0.1f);
        }
        else if (!falling && Mathf.Abs(movement.x) > 0f)
        {
            ChangeAnimation("Walk", 0.1f);
        }
        else if (currentAnimation != "Idle")
        {
            ChangeAnimation("Idle", 0.1f);
        }
    }

    //Check if current animation is the desired animation, crossfade to desired animation
    private void ChangeAnimation(string animation, float crossfade)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }
}
