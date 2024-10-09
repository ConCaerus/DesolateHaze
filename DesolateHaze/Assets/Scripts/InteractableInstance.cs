using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableInstance : MonoBehaviour {
    [SerializeField] List<UnityEvent> immediateSequence = new List<UnityEvent>();
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>();

    int seqInd = 0;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player")
            col.gameObject.GetComponent<PlayerInteraction>().setCurInteractable(this);
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Player")
            col.gameObject.GetComponent<PlayerInteraction>().setCurInteractable(null);
    }

    public void trigger() {
        immediateSequence[seqInd++].Invoke();
        if(seqInd >= immediateSequence.Count)
            seqInd = 0;
        for(int i = 0; i < delayedSequences.Count; i++) 
            StartCoroutine(delay(delayedSequences[i], secondsDelays[i]));
    }

    public void test() {
        Debug.Log(gameObject.name + ": " + "test");
    }
    public void dropPlayer() {
        PlayerMovement.I.setFalling();
    }
    public void stopPlayerMovement() {
        PlayerMovement.I.canMove = false;
    }
    public void startPlayerMovement() {
        PlayerMovement.I.canMove = true;
    }
    IEnumerator delay(UnityEvent e, float s) {
        yield return new WaitForSeconds(s);
        e.Invoke();
    }
}
