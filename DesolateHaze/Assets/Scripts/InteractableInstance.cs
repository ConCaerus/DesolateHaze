using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableInstance : MonoBehaviour {
    [SerializeField] List<UnityEvent> immediateSequence = new List<UnityEvent>();
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>();
    [SerializeField] List<UnityEvent> exitSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>();

    List<Coroutine> runners = new List<Coroutine>();

    int seqInd = 0;

    private void OnTriggerStay(Collider col) {
        if(col.gameObject.tag == "Player")
            PlayerInteraction.I.setCurInteractable(this);
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Player") {
            if(PlayerInteraction.I.getCurInteractable() == this)
                PlayerInteraction.I.setCurInteractable(null);
        }
    }

    private void OnDisable() {
        if(PlayerInteraction.I.getCurInteractable() == this)
            PlayerInteraction.I.setCurInteractable(null);
    }

    public void trigger() {
        if(!enabled) return;
        immediateSequence[seqInd++].Invoke();
        if(seqInd >= immediateSequence.Count)
            seqInd = 0;
        for(int i = 0; i < delayedSequences.Count; i++)
            runners.Add(StartCoroutine(delay(delayedSequences[i], secondsDelays[i])));
    }
    public void detrigger() {
        seqInd = 0;
        foreach(var i in runners)
            StopCoroutine(i);
        foreach(var i in exitSequences)
            i.Invoke();
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
    public void movePlayerHere() {
        PlayerMovement.I.canMove = false;
        PlayerMovement.I.resetSavedInput();
        PlayerMovement.I.transform.DOKill();
        PlayerMovement.I.transform.DOMoveX(transform.position.x, .25f);
    }
    IEnumerator delay(UnityEvent e, float s) {
        yield return new WaitForEndOfFrame();
        var rRef = runners[runners.Count - 1];
        yield return new WaitForSeconds(s);
        e.Invoke();
        runners.Remove(rRef);
    }
}
