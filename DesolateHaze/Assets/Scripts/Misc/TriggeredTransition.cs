using UnityEngine;
using DG.Tweening;

public class TriggeredTransition : MonoBehaviour {
    [SerializeField] Transform endPoint;
    [SerializeField] float transTime;

    [SerializeField] ASourceInstance asi;
    [SerializeField] AudioClip clip;

    public void trigger() {
        transform.DOKill();
        transform.DOMove(endPoint.position, transTime);
        transform.DORotate(endPoint.eulerAngles, transTime, RotateMode.FastBeyond360);

        if(asi != null && clip != null)
            asi.playSound(clip, false, false, 1f);
    }
}
