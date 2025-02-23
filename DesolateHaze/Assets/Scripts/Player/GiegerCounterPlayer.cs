using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiegerCounterPlayer : MonoBehaviour {
    [SerializeField] float fieldRange, closeRangePerc;
    [SerializeField] float minBtwTime, maxBtwTime, closeBtwTime;
    List<Transform> objs = new List<Transform>();

    float distPerc = 0f;

    [SerializeField] AudioPoolInfo sound;

    private void Awake() {
        foreach(var i in GameObject.FindGameObjectsWithTag("Irradiated")) {
            objs.Add(i.transform);
        }

        closeRangePerc /= fieldRange;   //  converts close range to a percentage

        StartCoroutine(player());
    }

    private void LateUpdate() {
        if(objs.Count > 0)
            manageSound();
    }

    void manageSound() {
        Transform close = objs[0].transform;
        var pPos = PlayerMovement.I.transform.position;
        var newDist = Vector3.Distance(close.position, pPos);

        foreach(var i in objs) {
            newDist = Vector3.Distance(i.position, pPos);
            if(newDist < Vector3.Distance(close.position, pPos)) {
                close = i;

                if(newDist < 1f) {
                    break;
                }
            }
        }
        newDist = Vector3.Distance(close.position, pPos);
        distPerc = Mathf.Clamp01(newDist / fieldRange);
    }

    IEnumerator player() {
        var btwTime = 0f;
        while(true) {
            while(distPerc > 0f) {
                btwTime = distPerc > closeRangePerc ? minBtwTime + (maxBtwTime - minBtwTime) * distPerc : closeBtwTime;
                AudioManager.I.playSound(sound, transform.position, 1f - distPerc);
                yield return new WaitForSeconds(btwTime);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
