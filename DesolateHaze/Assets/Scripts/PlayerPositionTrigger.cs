using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public void pausePlayerFallDamage(float seconds) {
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
}
