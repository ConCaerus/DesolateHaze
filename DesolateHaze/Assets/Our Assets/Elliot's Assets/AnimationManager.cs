using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

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
    public void CheckAnimation(Vector2 movement, PlayerMovement.pMovementState action)
    {
        switch(action) {
            case PlayerMovement.pMovementState.LadderClimbing:
                ChangeAnimation("Ladder_Climbing", 0.1f);
                if (Mathf.Abs(movement.y) > 0f)
                    animator.speed = 1f;
                if (Mathf.Abs(movement.y) < 0f)
                    animator.speed = -1f;
                if (Mathf.Abs(movement.y) == 0f)
                    animator.speed = 0f;
                break;
            case PlayerMovement.pMovementState.RopeClimbing:
                ChangeAnimation("Rope_Climbing", 0.1f);
                if (Mathf.Abs(movement.y) > 0f)
                    animator.speed = 1f;
                if (Mathf.Abs(movement.y) < 0f)
                    animator.speed = -1f;
                if (Mathf.Abs(movement.y) == 0f)
                    animator.speed = 0f;
                break;
            case PlayerMovement.pMovementState.LedgeClimbing:
                StartCoroutine(LedgeTime());
                break;
            case PlayerMovement.pMovementState.Pushing:
                ChangeAnimation("Push", 0.1f);
                if (Mathf.Abs(movement.x) == 0f)
                    animator.speed = 0f;
                if (PlayerMovement.I.facingRight == true)
                    animator.speed = 1f;
               // if (PlayerMovement.I.facingRight != true)
               //     animator.speed = -1f;
                break;
            case PlayerMovement.pMovementState.Falling:
                ChangeAnimation("Jump", 0.1f);
                break;
            case PlayerMovement.pMovementState.Walking:
                if (Mathf.Abs(movement.x) == 0f)
                    ChangeAnimation("Idle", 0.1f);
                else
                    ChangeAnimation("Walk", 0.1f);
                break;
            case PlayerMovement.pMovementState.None:
                ChangeAnimation("Idle", 0.1f);
                break;
        }
        //old gross elif
        /*
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
        else if (action == PlayerMovement.pMovementState.Falling || falling)
        {
            ChangeAnimation("Jump", 0.1f);
        }  
        else if (action == PlayerMovement.pMovementState.Walking && Mathf.Abs(movement.x) > 0f)
        {
            ChangeAnimation("Walk", 0.1f);
        }
        else if (currentAnimation != "Idle")
        {
            ChangeAnimation("Idle", 0.1f);
        }
        */
    }

    //Check if current animation is the desired animation, crossfade to desired animation
    public void ChangeAnimation(string animation, float crossfade) {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }

    IEnumerator LedgeTime() {
        ChangeAnimation("Ledge_Climbing", 0.1f);
        yield return new WaitForSeconds(2);
        PlayerMovement.I.resetMovement();
    }
}
