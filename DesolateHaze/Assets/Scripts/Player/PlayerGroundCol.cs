using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCol : MonoBehaviour {
    SphereCollider c;
    float fullSize;

    private void Start() {
        c = GetComponent<SphereCollider>();
        fullSize = c.radius;
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.touchedGround(col);
        c.radius = fullSize;
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.leftGround(col);
        c.radius = fullSize / 2f;
    }
}
