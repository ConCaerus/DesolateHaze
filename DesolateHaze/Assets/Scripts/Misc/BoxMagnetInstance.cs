using System.Collections;
using UnityEngine;

public class BoxMagnetInstance : MonoBehaviour {
    Rigidbody rb = null;
    Rigidbody magRb {
        get {
            return rb;
        }
        set {
            if(rb == value) return;
            if(rb != null) rb.isKinematic = false;
            rb = value;
            if(rb != null) rb.isKinematic = true;
            offset = rb.position - transform.position;
        }
    }
    Vector3 offset;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Ground" && col.gameObject.TryGetComponent<Rigidbody>(out var mrb)) {
            magRb = mrb;
        }
    }

    private void LateUpdate() {
        if(magRb != null)
            magRb.position = transform.position + offset;
    }
}
