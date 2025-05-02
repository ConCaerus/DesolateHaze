using DG.Tweening;
using UnityEngine;

public class ButtonInstance : MonoBehaviour {
    [SerializeField] Vector3 offset;
    [SerializeField] Transform buttonPart;
    Vector3 startPos;
    bool on;

    void Start() {
        startPos = buttonPart.localPosition;
        on = false;
    }
    public void ButtonDepressed(float dur) {
        if(!on) {
            buttonPart.DOKill();
            buttonPart.DOLocalMove(startPos + offset, dur);
            on = true;
        }
        else {
            buttonPart.DOKill();
            buttonPart.DOLocalMove(startPos, dur);
            on = false;
        }
    }
}
