using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RopeInstance : MonoBehaviour {
    //  ordered highest to lowest
    [SerializeField] List<Transform> segments = new List<Transform>();
    [SerializeField] List<Rigidbody> segRbs = new List<Rigidbody>();
    [SerializeField] float maxLinMag, maxAngMag;

    bool canHoldPlayer = true;

    Rigidbody prb;
    float movePerc;
    int curSegInd;

    Coroutine waiter = null;
    Coroutine clamper = null;

    Transform getClosestSeg(Vector3 point) {
        Transform temp = null;
        float minDist = Mathf.Infinity;
        foreach(var t in segments) {
            float dist = Vector3.Distance(t.position, point);
            if(dist < minDist) {
                temp = t;
                minDist = dist;
            }
        }
        return temp;
    }

    public Vector3 getGrabbedPos() {
        if(movePerc == 0f) return segments[curSegInd].position;

        //  moving up
        else if(movePerc > 0f) {
            if(curSegInd <= 0) return segments[curSegInd].position;
            return (segments[curSegInd].position * (1f - movePerc)) + (segments[curSegInd - 1].position * movePerc);
        }
        //  moving down
        else {
            if(curSegInd == segments.Count - 1) return segments[curSegInd].position;
            return (segments[curSegInd].position * (1f - Mathf.Abs(movePerc))) + (segments[curSegInd + 1].position * Mathf.Abs(movePerc));
        }
    }
    public void moveUp(float speed) {
        movePerc = Mathf.MoveTowards(movePerc, 1f, speed);
        if(movePerc >= 1f) {
            if(curSegInd == 0) {
                movePerc = 1f;
                return;
            }
            movePerc = 0f;
            curSegInd--;
        }
    }
    //  returns true if player needs to drop
    public bool moveDown(float speed) {
        movePerc = Mathf.MoveTowards(movePerc, -1f, speed);
        if(movePerc <= -1f && curSegInd < segments.Count - 1) {
            if(curSegInd == segments.Count - 2) {   //  no idea why it has to be -2 but it does
                movePerc = -1f;
                return true;
            }
            movePerc = 0f;
            curSegInd++;
        }
        return false;
    }

    public void holdPlayer(Rigidbody b) {
        if(waiter != null || !canHoldPlayer) return;
        //  sets can't collide with player
        foreach(var i in segRbs) {
            i.excludeLayers = LayerMask.GetMask(new string[] { "Player", "Rope", "GroundCollider" });
        }

        prb = b;
        curSegInd = segments.IndexOf(getClosestSeg(prb.transform.position));
        movePerc = 0f;

        if(clamper != null) StopCoroutine(clamper);
        clamper = StartCoroutine(clampVelocity());
    }
    public void dropPlayer() {
        if(waiter != null) return;
        waiter = StartCoroutine(dropWaiter());
        if(clamper != null) {
            StopCoroutine(clamper);
            clamper = null;
        }
    }
    IEnumerator dropWaiter() {
        yield return new WaitForSeconds(1f);
        //  sets can collide with player
        foreach(var i in segRbs) {
            i.excludeLayers = LayerMask.GetMask(new string[] { "Rope", "GroundCollider" });
        }
        waiter = null;
    }
    IEnumerator clampVelocity() {
        while(true) {
            foreach(var i in segRbs) {
                if(i.linearVelocity.magnitude > maxLinMag) i.linearVelocity = i.linearVelocity.normalized * maxLinMag;
                if(i.angularVelocity.magnitude > maxAngMag) i.angularVelocity = i.angularVelocity.normalized * maxAngMag;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void addSwingForce(Vector3 force) {
        var rb = segRbs[curSegInd];
        rb.AddForce(force, ForceMode.Acceleration);
    }
    public Vector2 getExitVel() {
        return getClosestSeg(prb.transform.position).GetComponent<Rigidbody>().linearVelocity;
    }

    public bool canHold() {
        return waiter == null && canHoldPlayer;
    }
    public void setCanHoldPlayer(bool b) {
        canHoldPlayer = b;
    }
}
