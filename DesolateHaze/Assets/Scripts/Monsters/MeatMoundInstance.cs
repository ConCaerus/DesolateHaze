using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class MeatMoundInstance : MonoBehaviour {
    [SerializeField] Transform pivot, tip;
    [SerializeField] Animator anim;
    [SerializeField] float strikeDist;
    [SerializeField] float rotSpeed;
    [SerializeField] float animSpeedMod;

    Transform playerTrans;

    Coroutine halter = null;

    bool hasKilledPlayer = false;

    private void OnTriggerStay(Collider other) {
        if(hasKilledPlayer) return;
        if(halter == null)
            movePivot();
        strikeCheck();
    }

    private void Start() {
        playerTrans = PlayerMovement.I.transform;
        anim.speed = animSpeedMod;
    }

    void movePivot() {
        var offset = playerTrans.position - pivot.position;
        var target = Vector3.forward * Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        if(target.z < 0f) target.z += 360f;
        pivot.localEulerAngles = Vector3.Lerp(pivot.localEulerAngles, target, rotSpeed * 100f * Time.deltaTime);
    }

    void strikeCheck() {
        if(halter == null && Vector3.Distance(pivot.position, playerTrans.position) < strikeDist) {
            anim.SetTrigger("strike");
            halter = StartCoroutine(halt());
        }
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

    IEnumerator halt() {
        yield return new WaitForSeconds(.5f * (1f / animSpeedMod));
        halter = null;
    }
}
