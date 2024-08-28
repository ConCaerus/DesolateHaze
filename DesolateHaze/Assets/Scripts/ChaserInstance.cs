using UnityEngine;
using System.Collections;

public class ChaserInstance : MonoBehaviour {
    [SerializeField] float speed, accSpeed;
    [SerializeField] Rigidbody rb;
    [SerializeField] bool chaseRight = true;

    Coroutine chaseWaiter = null, endWaiter = null;

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Player") {
            Debug.Log("Player died to monster: " + gameObject.name);
            PlayerMovement.I.canMove = false;
            TransitionCanvas.I.loadGameAfterDeath(2f);
            endChase(false);
        }
    }
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            Debug.Log("Player died to monster: " + gameObject.name);
            PlayerMovement.I.canMove = false;
            TransitionCanvas.I.loadGameAfterDeath(2f);
            endChase(false);
        }
    }

    public void triggerChase() {
        if(endWaiter != null) {
            StopCoroutine(endWaiter);
            endWaiter = null;
        }
        if(chaseWaiter != null) return;
        chaseWaiter = StartCoroutine(chaseSequence());
    }
    public void endChase(bool halt) {
        if(chaseWaiter != null) StopCoroutine(chaseWaiter);
        chaseWaiter = null;

        if(!halt) {
            if(endWaiter != null)
                endWaiter = StartCoroutine(endSequence());
        }
        else if(halt)
            rb.linearVelocity = Vector3.zero;
    }

    IEnumerator chaseSequence() {
        while(true) {
            yield return new WaitForFixedUpdate();
            var target = new Vector3(chaseRight ? speed : -speed, rb.linearVelocity.y);
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, target, accSpeed * Time.fixedDeltaTime * 100f);
        }
    }

    //  slows down to a halt over time
    IEnumerator endSequence() {
        yield return new WaitForFixedUpdate();
        rb.linearVelocity = new Vector3(chaseRight ? speed : -speed, rb.linearVelocity.y);
        while(rb.linearVelocity != Vector3.zero) {
            yield return new WaitForFixedUpdate();
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, Vector3.up * rb.linearVelocity.y, accSpeed * Time.fixedDeltaTime * 50f);
        }
        endWaiter = null;
    }
}
