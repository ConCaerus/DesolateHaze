using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HazardInstance : MonoBehaviour {
    public hazardType hType;
    [SerializeField] UnityEvent triggerEvents;
    [SerializeField] bool triggeredByMonsters = false;
    [SerializeField] AudioPoolInfo hazardSound;

    public enum hazardType {
        None, BarbedWire, Landmine
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Player") {
            hazardPlayerInteraction();
            triggerEvents.Invoke();
        }

        else if(col.gameObject.tag == "Monster" && triggeredByMonsters) 
            triggerEvents.Invoke();
    }
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            hazardPlayerInteraction();
            triggerEvents.Invoke();
        }

        else if(col.gameObject.tag == "Monster" && triggeredByMonsters)
            triggerEvents.Invoke();
    }

    private void Start() {
        AudioManager.I.initSound(hazardSound);
    }

    public void hazardPlayerInteraction() {
        switch(hType) {
            case hazardType.BarbedWire:
                PlayerMovement.I.beKilled();
                Debug.Log("Player Died to Hazard: " + hType.ToString() + " " + gameObject.name);
                TransitionCanvas.I.loadGameAfterDeath(1.5f);
                break;

            case hazardType.Landmine:
                PlayerMovement.I.beKilled();
                PlayerMovement.I.rb.AddExplosionForce(1000f, transform.position + Vector3.down, 3f);
                Debug.Log("Player Died to Hazard: " + hType.ToString() + " " + gameObject.name);
                TransitionCanvas.I.loadGameAfterDeath(3f);
                break;
        }
    }

    public void playHazardSound() {
        AudioManager.I.playSound(hazardSound, transform.position, 1f);
    }
}
