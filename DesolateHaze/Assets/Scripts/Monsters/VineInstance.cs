using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class VineInstance : MonoBehaviour {
    [SerializeField] float sightRange, repelRange, totalRange;
    [SerializeField] float normOffset, woundOffset, repelledOffset;
    [SerializeField] float windTime, strikeTime, returnTime;
    [SerializeField] Transform attVine, attVineRot;
    [SerializeField] List<Transform> repellants = new List<Transform>();
    [SerializeField] bool hardTracking = true;
    [SerializeField] bool singleShot = false;
    bool canTrack = true;

    bool r = false;
    bool repelled {
        get { return r; }
        set {
            if(r == value) return;
            r = value;
            attVine.DOKill();
            attVine.DOLocalMoveZ(r ? repelledOffset : normOffset, .5f);
        }
    }

    Transform playerTrans = null;
    Coroutine attacker = null;

    private void LateUpdate() {
        attackCheck();
    }
    private void OnDisable() {
        attVine.DOKill();
    }

    void attackCheck() {
        if(playerTrans == null)
            playerTrans = PlayerMovement.I.transform;

        //  checks if repelled
        bool tempRepelled = false;
        foreach(var i in repellants) {
            if(i.gameObject.activeInHierarchy && Mathf.Abs(i.transform.position.x - transform.position.x) < repelRange) {
                tempRepelled = true;
                if(attacker != null) {
                    StopCoroutine(attacker);
                    attacker = null;
                }
                break;
            }
        }
        repelled = tempRepelled;

        var d = Mathf.Abs(playerTrans.position.x - attVineRot.position.x);
        //  checks if close enough to animate looking at the player
        if(canTrack && d < totalRange)
            attVineRot.LookAt(playerTrans.position);

        //  checks if close enough to attack the player
        if(!repelled && attacker == null && d < sightRange) {
            attacker = StartCoroutine(attackWaiter());
        }
    }

    public void killPlayer() {
        PlayerMovement.I.canMove = false;
        PlayerMovement.I.rb.isKinematic = true;
        if(attacker != null) {
            StopCoroutine(attacker);
            attacker = null;
        }
        attVine.DOKill();
        attVine.DOLocalMoveZ(normOffset, returnTime);
        StartCoroutine(killPlayerWaiter(attVine.position - playerTrans.position));

        TransitionCanvas.I.loadGameAfterDeath(2.5f);
    }

    IEnumerator attackWaiter() {
        attVine.DOKill();

        //  visually winds up for a strike
        attVine.DOLocalMoveZ(woundOffset, windTime);
        canTrack = hardTracking;
        yield return new WaitForSeconds(windTime + .1f);

        //  strikes out towards the player's current location
        var attOffset = Vector3.Distance(playerTrans.position, attVineRot.position);
        attVine.DOLocalMoveZ(attOffset, strikeTime);
        yield return new WaitForSeconds(strikeTime + .1f);

        //  returns to rest
        attVine.DOLocalMoveZ(normOffset, returnTime);
        yield return new WaitForSeconds(returnTime + .1f);
        if(singleShot)
            enabled = false;
        canTrack = true;
        attacker = null;
    }
    IEnumerator killPlayerWaiter(Vector3 hitOffset) {
        while(true) {
            playerTrans.position = attVine.position + hitOffset;
            yield return new WaitForEndOfFrame();
        }
    }
}
