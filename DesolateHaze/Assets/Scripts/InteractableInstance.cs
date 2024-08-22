using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractableInstance : MonoBehaviour {
    [SerializeField] UnityEvent sequence = new UnityEvent();

    private void OnEnable() {
        StartCoroutine(adder());
    }
    private void OnDisable() {
        if(PlayerInteraction.I != null && PlayerInteraction.I.interactables.Contains(this))
            PlayerInteraction.I.interactables.Remove(this);
    }

    public void trigger() {
        sequence.Invoke();
    }

    public void test() {
        Debug.Log(gameObject.name + ": " + "test");
    }

    IEnumerator adder() {
        do yield return new WaitForEndOfFrame();
        while(PlayerInteraction.I == null);
        PlayerInteraction.I.interactables.Add(this);
    }
}
