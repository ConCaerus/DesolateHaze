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
                Debug.Log("Player Died to Hazard: " + hType.ToString() + " " + gameObject.name);
                TransitionCanvas.I.loadGameAfterDeath(1.5f);
                break;

            case hazardType.Landmine:
                pm.canMove = false;
                pm.rb.AddExplosionForce(1000f, transform.position + Vector3.down, 3f);
                Debug.Log("Player Died to Hazard: " + hType.ToString() + " " + gameObject.name);
                TransitionCanvas.I.loadGameAfterDeath(3f);
                break;
        }

        triggerEvents.Invoke();
    }
}
