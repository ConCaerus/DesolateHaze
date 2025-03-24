using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagnetCraneMagnetCollider : MonoBehaviour {
    [SerializeField] List<Collider> canCollides = new List<Collider>();

    private void OnTriggerEnter(Collider col) {
        if(canCollides.Contains(col)) {
            Destroy(col.GetComponentInParent<VehicleInstance>());
            Destroy(col.GetComponentInParent<Rigidbody>());
            col.transform.parent.parent = transform;
            canCollides.Remove(col);
            col.enabled = false;
            Debug.Log("here");
        }
    }
}
