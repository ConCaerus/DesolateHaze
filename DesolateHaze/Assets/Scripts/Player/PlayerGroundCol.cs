using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCol : MonoBehaviour {
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.touchedGround(col);
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.leftGround(col);
    }
}
