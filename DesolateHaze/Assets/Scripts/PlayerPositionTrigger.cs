using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerPositionTrigger : MonoBehaviour {
    [SerializeField] UnityEvent events;
    [SerializeField] List<UnityEvent> delayedSequences = new List<UnityEvent>();
    [SerializeField] List<float> secondsDelays = new List<float>();
    [SerializeField] bool singleUse = true;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            events.Invoke();
            for(int i = 0; i < delayedSequences.Count; i++)
                StartCoroutine(delay(delayedSequences[i], secondsDelays[i]));

            if(singleUse)
                gameObject.SetActive(false);
        }
    }

    public void setPlayerSpeedMod(float mod) {
        PlayerMovement.I.speedMod = mod;
    }
    public void setPlayerFalling() {
        PlayerMovement.I.setNewState(PlayerMovement.pMovementState.Falling);
    }
    public void loadLevel2() {
        SceneManager.LoadScene("Onsite");
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
