using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerPositionTrigger : MonoBehaviour {
    [SerializeField] UnityEvent events, exitEvents;
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>(), exitDelayedSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>(), exitSecondsDelay = new List<float>();
    [SerializeField] bool singleUse = true;

    [SerializeField] AudioPoolInfo sound;

    private void Start() {
        AudioManager.I.initSound(sound);
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            events.Invoke();
            for(int i = 0; i < delayedSequences.Count; i++)
                StartCoroutine(delay(delayedSequences[i], secondsDelays[i]));

            if(singleUse)
                gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Player") {
            exitEvents.Invoke();
            for(int i = 0; i < exitDelayedSequences.Count; i++)
                StartCoroutine(delay(exitDelayedSequences[i], exitSecondsDelay[i]));
        }
    }

    public void setPlayerSpeedMod(float mod) {
        PlayerMovement.I.speedMod = mod;
    }
    public void setPlayerFalling() {
        PlayerMovement.I.setNewState(PlayerMovement.pMovementState.Falling);
    }
    public void loadNextArea() {
        TransitionCanvas.I.loadNextArea(CheckpointManager.I.aType);
    }
    public void killPlayer(float waitTime) {
        PlayerMovement.I.beKilled();
        TransitionCanvas.I.loadGameAfterDeath(waitTime);
    }
    public void setClosestPushing() {
        PlayerMovement.I.closePushing = GetComponentInParent<Rigidbody>();
    }
    public void unsetClosestPushing() {
        if(PlayerMovement.I.closePushing = GetComponentInParent<Rigidbody>())
            PlayerMovement.I.closePushing = null;
    }

    public void setPlayerContaminated(bool b) {
        PlayerMovement.I.isContaminated = b;
    }

    public void playSound(Transform origin) {
        AudioManager.I.playSound(sound, origin.position, 1f);
    }

    public void pauseFallDamage() {
        PlayerMovement.I.canTakeFallDamage = false;
    }
    public void resumeFallDamage() {
        PlayerMovement.I.canTakeFallDamage = true;
    }
    public void pauseFallDamageForTime(float seconds) {
        StartCoroutine(pauseFallDamage(seconds));
    }
    IEnumerator pauseFallDamage(float seconds) {
        PlayerMovement.I.canTakeFallDamage = false;
        yield return new WaitForSeconds(seconds);
        PlayerMovement.I.canTakeFallDamage = true;
    }
    IEnumerator delay(UnityEvent e, float s) {
        yield return new WaitForSeconds(s);
        e.Invoke();
    }

    public void testWarn() {
        Debug.Log("warn");
    }
}
