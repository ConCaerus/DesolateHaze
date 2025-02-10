using System.Collections;
using UnityEngine;

public class MeatMoundInstance : MonoBehaviour {
    [SerializeField] Transform pivot, tip;
    [SerializeField] Animator anim;
    [SerializeField] float strikeDist;

    Transform playerTrans;

    bool hasKilledPlayer = false;

    private void OnTriggerStay(Collider other) {
        if(hasKilledPlayer) return;
        movePivot();
        strikeCheck();
    }

    private void Start() {
        playerTrans = PlayerMovement.I.transform;
    }

    void movePivot() {
        var offset = playerTrans.position - pivot.position;
        pivot.localEulerAngles = Vector3.forward * Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }

    void strikeCheck() {
        if(Vector3.Distance(pivot.position, playerTrans.position) < strikeDist)
            anim.SetTrigger("strike");
    }

    public void killPlayer() {
        hasKilledPlayer = true;
        anim.ResetTrigger("strike");
        PlayerMovement.I.beKilled();
        StartCoroutine(killPlayerWaiter(tip.position - playerTrans.position));

        TransitionCanvas.I.loadGameAfterDeath(2.5f);
    }

    IEnumerator killPlayerWaiter(Vector3 hitOffset) {
        while(true) {
            playerTrans.position = tip.position + hitOffset;
            yield return new WaitForEndOfFrame();
        }
    }
}
