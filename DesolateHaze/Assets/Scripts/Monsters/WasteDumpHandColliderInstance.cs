using UnityEngine;

public class WasteDumpHandColliderInstance : MonoBehaviour {
    bool canKill = true;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Box")
            canKill = false;
        else if(col.gameObject.tag == "Player" && canKill) {
            PlayerMovement.I.canMove = false;
            TransitionCanvas.I.loadGameAfterDeath(1.5f);
        }
    }
    private void OnTriggerStay(Collider col) {
        if(col.gameObject.tag == "Box")
            canKill = false;
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Box")
            canKill = true;
    }
}
