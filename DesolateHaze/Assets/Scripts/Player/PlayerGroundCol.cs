using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCol : MonoBehaviour {
    private void OnTriggerEnter(Collider col) {
        PlayerMovement.I.touchedGround(col);
    }
    private void OnTriggerExit(Collider col) {
        PlayerMovement.I.leftGround(col);
    }
}
