using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableInstance : MonoBehaviour {
    [SerializeField] UnityEvent immediateSequence = new UnityEvent();
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>();

    private void OnEnable() {
        StartCoroutine(adder());
    }
    private void OnDisable() {
        if(PlayerInteraction.I != null && PlayerInteraction.I.interactables.Contains(this))
            PlayerInteraction.I.interactables.Remove(this);
    }

    public void trigger() {
        immediateSequence.Invoke();
        for(int i = 0; i < delayedSequences.Count; i++) 
            StartCoroutine(delay(delayedSequences[i], secondsDelays[i]));
    }

    public void test() {
        Debug.Log(gameObject.name + ": " + "test");
    }
    public void dropPlayer() {
        PlayerMovement.I.setFalling();
    }

    IEnumerator adder() {
        do yield return new WaitForEndOfFrame();
        while(PlayerInteraction.I == null);
        PlayerInteraction.I.interactables.Add(this);
    }
    IEnumerator delay(UnityEvent e, float s) {
        yield return new WaitForSeconds(s);
        e.Invoke();
    }
}
