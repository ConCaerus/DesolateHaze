using DG.Tweening;
using UnityEngine;

public class DoorInstance : MonoBehaviour {
    [SerializeField] Vector3 endOffset;
    [SerializeField] bool localSpace = false;
    Vector3 startPos;

    private void Awake() {
        startPos = localSpace ? transform.localPosition : transform.position;
    }

    public void moveToEnd(float dur) {
        transform.DOKill();
        if(localSpace) transform.DOLocalMove(startPos + endOffset, dur);
        else transform.DOMove(startPos + endOffset, dur);
    }
    public void moveToStart(float dur) {
        transform.DOKill();
        if(localSpace) transform.DOLocalMove(startPos, dur);
        else transform.DOMove(startPos, dur);
    }

    public Vector3 getOffset() {
        return endOffset;
    }
}
