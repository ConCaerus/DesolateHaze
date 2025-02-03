using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ToppleableInstance : MonoBehaviour {
    [SerializeField] UnityEvent toppleEvents;
    [SerializeField] bool toppleRight = true;
    [SerializeField] float toppleRange;
    [SerializeField] float toppleTime;

    Vector3 toppleOrigin;
    Transform tweener = null;

    bool toppling = false;

    private void Start() {
        tweener = new GameObject().transform;
        tweener.transform.parent = transform;
        tweener.transform.localPosition = Vector3.zero;
    }

    public void startTopple() {
        if(toppling) return;
        toppling = true;
        toppleOrigin = transform.position;

        tweener.DOKill();
        tweener.DOLocalMoveX(1f, toppleTime).OnComplete(() => {
            toppleEvents.Invoke();
        }).OnUpdate(() => {
            var topOff = Vector3.right * toppleRange * tweener.localPosition.x * (toppleRight ? 1f : -1f);
            transform.position = toppleOrigin + topOff;
        });
    }
    public void stopTopple() {
        toppling = false;
        tweener.DOKill();
        tweener.localPosition = Vector3.zero;
        transform.position = toppleOrigin;
    }
}
