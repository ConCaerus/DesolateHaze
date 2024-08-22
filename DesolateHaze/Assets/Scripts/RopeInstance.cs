using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RopeInstance : MonoBehaviour {
    //  ordered highest to lowest
    [SerializeField] List<Transform> segments = new List<Transform>();
    [SerializeField] List<Rigidbody> segRbs = new List<Rigidbody>();

    Rigidbody prb;
    float movePerc;
    int curSegInd;

    Coroutine waiter = null;

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
    public void moveDown(float speed) {
        movePerc = Mathf.MoveTowards(movePerc, -1f, speed);
        if(movePerc <= -1f && curSegInd < segments.Count - 1) {
            if(curSegInd == segments.Count - 1) {
                movePerc = -1f;
                return;
            }
            movePerc = 0f;
            curSegInd++;
        }
    }

    public void holdPlayer(Rigidbody b) {
        if(waiter != null) return;
        //  sets can't collide with player
        foreach(var i in segRbs) {
            i.excludeLayers = LayerMask.GetMask(new string[] { "Player", "Rope" });
        }

        prb = b;
        curSegInd = segments.IndexOf(getClosestSeg(prb.transform.position));
        movePerc = 0f;
    }
    public void dropPlayer() {
        if(waiter != null) return;
        waiter = StartCoroutine(dropWaiter());
        //  sets can collide with player
        foreach(var i in segRbs) {
            i.excludeLayers = LayerMask.GetMask("Rope");
        }
    }
    IEnumerator dropWaiter() {
        yield return new WaitForSeconds(2f);
        waiter = null;
    }

    public void addSwingForce(Vector3 force) {
        var rb = segRbs[curSegInd];
        rb.AddForce(force, ForceMode.Acceleration);
    }
    public Vector2 getExitVel() {
        return getClosestSeg(prb.transform.position).GetComponent<Rigidbody>().linearVelocity;
    }

    public bool canHold() {
        return waiter == null;
    }
}
