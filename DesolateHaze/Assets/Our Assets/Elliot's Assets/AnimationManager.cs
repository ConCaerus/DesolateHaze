using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private string currentAnimation = "";
    private Vector2 movement = Vector2.zero;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ChangeAnimation("Idle");
    }

    
    private void FixedUpdate()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        CheckAnimation();
    }

    private void CheckAnimation()
    {
        if (movement.y == 1 || movement.y == -1)
        {
            ChangeAnimation("Walk", 0.2f);
            Debug.Log("Walking!!");
        }
        else
        {
            ChangeAnimation("Idle", 0.2f);
        }
    }
    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
            Debug.Log("Animation change!");
        }
    }
}
