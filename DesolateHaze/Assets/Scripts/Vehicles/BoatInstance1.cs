using UnityEngine;

public class BoatInstance : MonoBehaviour {
    bool ow = false;
    public bool onWater {
        get { return ow; }
        set { 
            ow = value;
            if(ow)
                Invoke("startRotating", .5f);
        }
    }
    [SerializeField] float boatRot;
    [SerializeField] float speed, accSpeed;
    [SerializeField] bool moveRight;

    [SerializeField] Rigidbody rb;
    [SerializeField] Collider mainCol;

    bool rotate = false;


    private void FixedUpdate() {
        beBoat();
    }

    void startRotating() {
        rotate = true;
    }

    void beBoat() {
        if(onWater) {
            //  starts moving
            Vector3 target = Vector3.up * rb.linearVelocity.y;
            if(PlayerMovement.I.getUsedGround() == mainCol) {
                target = Vector3.right * speed * (moveRight ? 1f : -1f);
                rb.freezeRotation = true;
                if(PlayerMovement.I.getInheritRb() != rb)
                    PlayerMovement.I.setInheritRb(rb);
            }
            else {
                if(PlayerMovement.I.getInheritRb() == rb)
                    PlayerMovement.I.setInheritRb(null);
            }
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target, accSpeed * Time.fixedDeltaTime);

            //  rotates into boatable
            if(rotate) {
                target = Vector3.forward * boatRot;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(target), accSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
