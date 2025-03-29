using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : Singleton<AnimationManager>
{
    [SerializeField] Animator animator;
    private string currentAnimation = "";
    public bool isFalling = false;
    public bool oneSec = false;

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
    public void CheckAnimation(Vector2 movement, PlayerMovement.pMovementState action, bool grounded)
    {
        //if (grounded)
        //    isFalling = false;
        switch (action) {
            case PlayerMovement.pMovementState.LadderClimbing:
                animator.speed = 1f;
                if (Mathf.Abs(movement.y) == 0f)
                    animator.speed = 0f;
                if (Mathf.Abs(movement.y) > 0f)
                    ChangeAnimation("Ladder_Climbing", 0.1f);
                if (Mathf.Abs(movement.y) < 0f)
                    ChangeAnimation("Ladder_Climbing_Down", 0.1f);
                break;
            case PlayerMovement.pMovementState.RopeClimbing:
                animator.speed = 1f;
                if (Mathf.Abs(movement.y) == 0f)
                    animator.speed = 0f;
                if (Mathf.Abs(movement.y) > 0f)
                    ChangeAnimation("Rope_Climbing", 0.1f);
                if (Mathf.Abs(movement.y) < 0f)
                    ChangeAnimation("Rope_Climbing_Down", 0.1f);
                break;
            case PlayerMovement.pMovementState.LedgeClimbing:
                animator.speed = 1f;
                StartCoroutine(LedgeTime());
                break;
            case PlayerMovement.pMovementState.Pushing:
                animator.speed = 1f;
                if (Mathf.Abs(movement.x) == 0f)
                    animator.speed = 0f;
                ChangeAnimation(PlayerMovement.I.checkDirection(), 0.1f);
                break;
            case PlayerMovement.pMovementState.Driving:
                animator.speed = 1f;
                ChangeAnimation("Driving", 0.1f);
                break;
            case PlayerMovement.pMovementState.Crawling:
                animator.speed = 1f;
                if (Mathf.Abs(movement.x) == 0f)
                    animator.speed = 0f;
                if (Mathf.Abs(movement.x) > 0f)
                    ChangeAnimation("Crawl", 0.1f);
                if (Mathf.Abs(movement.x) < 0f)
                    ChangeAnimation("Crawl_Back", 0.1f);
                break;
            case PlayerMovement.pMovementState.Falling:
                animator.speed = 1f;
                if (!isFalling)
                    StartCoroutine(LongFall());
                //if (currentAnimation != "Falling")
                //   ChangeAnimation("Jump", 0.1f);
                break;
            case PlayerMovement.pMovementState.Walking:
                //animator.speed = 1f;
                /*if (currentAnimation == "Falling")
                {
                    ChangeAnimation("Landing", 0.1f);
                    StartCoroutine(WaitForAnim(currentAnimation, 1f));
                    break;
                } */
                if (Mathf.Abs(movement.x) == 0f)
                    ChangeAnimation("Idle", 0.1f);
                else
                    ChangeAnimation("Walk", 0.1f);
                break;
            case PlayerMovement.pMovementState.None:
                animator.speed = 1f;
                if (currentAnimation == "Falling")
                {
                    StartCoroutine(WaitForAnim(currentAnimation, 1f));
                    break;
                }
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

    public void ChangeAnimation(string animation, float crossfade) {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }

    IEnumerator LedgeTime() {
        ChangeAnimation("Ledge_Climbing", 0.1f);
        yield return new WaitForSeconds(1);
        PlayerMovement.I.resetMovement();
        yield break;
    }

    IEnumerator LongFall() {
        isFalling = true;
        ChangeAnimation("Jump", 0.1f);
        StartCoroutine(SecWaiter());
        while (currentAnimation == "Jump")
        {
            if (oneSec)
            {
                ChangeAnimation("Falling", 0.1f);
            }
        }
        /*       WaitUntil wait = new WaitUntil(() => PlayerMovement.I.curState != PlayerMovement.pMovementState.Falling);
               yield return wait;
               if (!PlayerMovement.I.canMove)
                   PlayerMovement.I.canMove = false;
               ChangeAnimation("Landing", 0.1f);
               Debug.Log("Landing!");
               yield return new WaitForSeconds(1f);
               PlayerMovement.I.canMove = true;
               PlayerMovement.I.resetMovement(); */
        isFalling = false;
        yield break;
    }

    IEnumerator SecWaiter()
    {
        yield return new WaitForSeconds(1f);
        oneSec = true;
        yield break;
    }

    IEnumerator WaitForAnim(string Animation, float time)
    {
        if (PlayerMovement.I.canMove)
            PlayerMovement.I.canMove = false;
        ChangeAnimation(Animation, 0.1f);
        yield return new WaitForSeconds(time);
        PlayerMovement.I.canMove = true;
        yield break;
    }
}