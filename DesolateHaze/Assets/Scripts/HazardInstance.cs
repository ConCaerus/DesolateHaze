using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HazardInstance : MonoBehaviour {
    public hazardType hType;
    [SerializeField] UnityEvent triggerEvents;

    public enum hazardType {
        None, BarbedWire, Landmine
    }

    public void triggerHazard(PlayerMovement pm) {
        switch(hType) {
            case hazardType.BarbedWire:
                pm.canMove = false;
                StartCoroutine(deathWaiter(1.5f));
                break;

            case hazardType.Landmine:
                pm.canMove = false;
                pm.rb.AddExplosionForce(1000f, transform.position, 3f);
                StartCoroutine(deathWaiter(3f));
                break;
        }

        triggerEvents.Invoke();
    }

    IEnumerator deathWaiter(float delay) {
        yield return new WaitForSeconds(delay);
        TransitionCanvas.I.loadGame();
    }
}
