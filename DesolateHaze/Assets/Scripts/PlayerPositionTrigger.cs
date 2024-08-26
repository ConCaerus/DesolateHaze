using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPositionTrigger : MonoBehaviour {
    [SerializeField] UnityEvent events;
    [SerializeField] bool singleUse = true;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            events.Invoke();
            if(singleUse)
                enabled = false;
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
}
