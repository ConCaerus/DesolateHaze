using UnityEngine;
using DG.Tweening;

public class TriggeredTransition : MonoBehaviour {
    [SerializeField] Transform endPoint;
    [SerializeField] float transTime;

    public void trigger() {
        transform.DOKill();
        transform.DOMove(endPoint.position, transTime);
        transform.DORotate(endPoint.eulerAngles, transTime, RotateMode.FastBeyond360);
    }
}
