using UnityEngine;

public class WaterInstance : MonoBehaviour {
    [SerializeField] float buoyancy;
    [SerializeField] Collider mainCol;

    private void OnTriggerEnter(Collider col) {
        if(col.TryGetComponent<BoatInstance>(out var b))
            b.onWater = true;
    }

    private void OnTriggerStay(Collider col) {
        if(col.gameObject.tag == "Box" && col.TryGetComponent<Rigidbody>(out var rb)) {
            if(col.bounds.center.y < mainCol.bounds.max.y) 
                rb.linearVelocity += Vector3.up * buoyancy * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.TryGetComponent<BoatInstance>(out var b))
            b.onWater = false;
    }
}
