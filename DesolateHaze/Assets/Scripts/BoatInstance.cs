using UnityEngine;

public class BoatInstance : MonoBehaviour {
    public bool onWater = false;
    [SerializeField] float boatRot;
    [SerializeField] float speed, accSpeed;
    [SerializeField] bool moveRight;

    [SerializeField] Rigidbody rb;
    [SerializeField] Collider mainCol;


    private void FixedUpdate() {
        beBoat();
    }

    void beBoat() {
        if(onWater) {
            Vector3 target = Vector3.up * rb.linearVelocity.y;
            if(PlayerMovement.I.getUsedGround() == mainCol) {
                target = Vector3.Lerp(rb.linearVelocity, Vector3.right * speed * (moveRight ? 1f : -1f), accSpeed * Time.fixedDeltaTime);
                rb.freezeRotation = true;
            }
            rb.linearVelocity = target;

            transform.localEulerAngles = Vector3.forward * boatRot;
        }
    }
}
