using DG.Tweening;
using UnityEngine;

public class ButtonInstance : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    Vector3 startPos;
    bool on;

    void Start()
    {
        startPos = transform.position;
        on = false;
    }
    public void ButtonDepressed(float dur)
    {
        if (!on)
        {
            transform.DOKill();
            transform.DOMove(startPos + offset, dur);
            on = true;
        }
        else
        {
            transform.DOKill();
            transform.DOMove(startPos, dur);
            on = false;
        }
    }
}
