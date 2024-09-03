using UnityEngine;

public class CrowInstance : MonoBehaviour {
    [SerializeField] Vector2 exitVel;
    [SerializeField] Rigidbody rb;

    public void flyOff() {
        rb.linearVelocity = exitVel;
        Destroy(gameObject, 1f);
    }
}
