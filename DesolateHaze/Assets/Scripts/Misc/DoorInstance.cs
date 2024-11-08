using DG.Tweening;
using UnityEngine;

public class DoorInstance : MonoBehaviour {
    [SerializeField] Vector3 endOffset;
    Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    public void moveToEnd(float dur) {
        transform.DOKill();
        transform.DOMove(startPos + endOffset, dur);
    }
    public void moveToStart(float dur) {
        transform.DOKill();
        transform.DOMove(startPos, dur);
    }
}
