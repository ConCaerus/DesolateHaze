using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerPositionTrigger : MonoBehaviour {
    [SerializeField] UnityEvent events, exitEvents;
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>(), exitDelayedSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>(), exitSecondsDelay = new List<float>();
    [SerializeField] bool singleUse = true;
    [SerializeField] Collider mainCol;

    [SerializeField] AudioPoolInfo sound;

    private void Start() {
        if(mainCol == null) TryGetComponent(out mainCol); 
        AudioManager.I.initSound(sound);
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            events.Invoke();
            for(int i = 0; i < delayedSequences.Count; i++)
                StartCoroutine(delay(delayedSequences[i], secondsDelays[i]));

            if(singleUse)
                mainCol.enabled = false;
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

    //  cam changers
    public void anchorCamera(float fov = 80) {
        if(transform.childCount == 0) {
            Debug.LogError("Create a child for the PlayerPositionTrigger that is anchoring the Camera that is in the anchored position.");
            return;
        }
        CameraMovement.I.setAnchorPoint(transform.GetChild(0).position, fov);
    }
    public void unanchorCamera() {
        CameraMovement.I.unAnchorPoint();
    }
    public void setCamLookingAt(Transform lookAt) {
        CameraMovement.I.lookAtTrans = lookAt;
    }
    public void unsetCamLookat() {
        CameraMovement.I.lookAtTrans = null;
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
