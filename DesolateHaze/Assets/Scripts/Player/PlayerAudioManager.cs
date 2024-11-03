using System.Collections;
using UnityEngine;

public class PlayerAudioManager : Singleton<PlayerAudioManager> {
    [SerializeField] AudioPoolInfo walkSound;

    Coroutine walker = null;

    private void Start() {
        AudioManager.I.initSound(walkSound);
    }

    public void startWalking(float timeBtwSteps) {
        if(walker == null)
            walker = StartCoroutine(walkerWaiter(timeBtwSteps));
    }
    public void stopWalking() {
        if(walker != null) {
            StopCoroutine(walker);
            walker = null;
        }
    }

    IEnumerator walkerWaiter(float timeBtwSteps) {
        while(true) {
            if(PlayerMovement.I.grounded)
                AudioManager.I.playSound(walkSound, transform.position, 1f);
            yield return new WaitForSeconds(timeBtwSteps);
        }
    }
}
