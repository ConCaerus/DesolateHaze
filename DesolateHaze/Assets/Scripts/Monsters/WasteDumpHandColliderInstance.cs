using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WasteDumpHandColliderInstance : MonoBehaviour {
    [SerializeField] DoorInstance hand;
    bool canKill = true;

    Coroutine killWaiter = null;

    private void OnTriggerEnter(Collider col) {
        if(killWaiter != null) return;
        if(col.gameObject.tag == "Box")
            canKill = false;
        if(col.gameObject.tag == "Player" && canKill) {
            canKill = false;
            killWaiter = StartCoroutine(deathSequence());
        }
    }
    private void OnTriggerStay(Collider col) {
        if(killWaiter != null) return;
        if(col.gameObject.tag == "Box")
            canKill = false;
        if(col.gameObject.tag == "Player" && canKill) {
            canKill = false;
            StartCoroutine(deathSequence());
        }
    }
    private void OnTriggerExit(Collider col) {
        if(killWaiter != null) return;
        if(col.gameObject.tag == "Box")
            canKill = true;
    }

    IEnumerator deathSequence() {
        PlayerMovement.I.canMove = false;
        TransitionCanvas.I.loadGameAfterDeath(2f);
        hand.moveToEnd(.15f);
        yield return new WaitForSeconds(.15f);
        PlayerMovement.I.rb.isKinematic = true;
        PlayerMovement.I.GetComponent<Collider>().enabled = false;
        PlayerMovement.I.enabled = false;
        PlayerMovement.I.transform.DOMove((Vector2)hand.transform.position, .19f);
        yield return new WaitForSeconds(.2f);
        hand.moveToStart(.25f);
        PlayerMovement.I.transform.DOMoveZ(-hand.getOffset().z, .25f);
    }
}
