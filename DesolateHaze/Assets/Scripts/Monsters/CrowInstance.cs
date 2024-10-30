using UnityEngine;

public class CrowInstance : MonoBehaviour {
    [SerializeField] Vector2 exitVel;
    [SerializeField] Rigidbody rb;

    [SerializeField] AudioPoolInfo caw;

    public void flyOff() {
        rb.linearVelocity = exitVel;
        Destroy(gameObject, 1f);

        AudioManager.I.playSound(caw, transform.position, 1f);
    }
}
