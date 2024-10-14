using UnityEngine;

public class WagonInstance : MonoBehaviour {
    [SerializeField] float speed, accSpeed;
    [SerializeField] bool moveRight;

    [SerializeField] Rigidbody rb;
    [SerializeField] Collider mainCol;


    private void FixedUpdate() {
        beBoat();
    }

    void beBoat() {
        //  starts moving
        Vector3 target = Vector3.zero;
        if(PlayerMovement.I.getUsedGround() == mainCol) {
            target = Vector3.right * speed * (moveRight ? 1f : -1f);
            if(PlayerMovement.I.getInheritRb() != rb)
                PlayerMovement.I.setInheritRb(rb);
        }
        else {
            if(PlayerMovement.I.getInheritRb() == rb)
                PlayerMovement.I.setInheritRb(null);
        }
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target, accSpeed * Time.fixedDeltaTime);
    }
}
