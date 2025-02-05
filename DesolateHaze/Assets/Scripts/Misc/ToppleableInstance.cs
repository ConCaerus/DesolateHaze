using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ToppleableInstance : MonoBehaviour {
    [SerializeField] UnityEvent toppleEvents;
    [SerializeField] bool toppleRight = true;
    [SerializeField] bool push = true;
    [SerializeField] float toppleRange;
    [SerializeField] float toppleTime;

    Vector3 toppleOrigin;
    Transform tweener = null;

    bool toppling = false;

    Coroutine checker = null;

    private void Start() {
        tweener = new GameObject().transform;
        tweener.transform.parent = transform;
        tweener.transform.localPosition = Vector3.zero;
    }

    public void startTopple() {
        if(toppling) return;
        if(checker == null) checker = StartCoroutine(toppleChecker());
        toppling = true;
        toppleOrigin = transform.position;


    }
    public void stopTopple() {
        toppling = false;
        if(checker != null) {
            StopCoroutine(checker);
            checker = null;
        }
        tweener.DOKill();
        tweener.localPosition = Vector3.zero;
        transform.position = toppleOrigin;
    }

    IEnumerator toppleChecker() {
        bool curToppling = false;
        while(true) {
            bool shouldTopple = true;
            //  checks if toppling the right way
            var offset = transform.position.x - PlayerMovement.I.transform.position.x;
            if(toppleRight == offset > 0f) {
                shouldTopple = false;
            }

            //  checks if pushing / pulling the right way
            else if(push == (offset > 0f != PlayerMovement.I.getSavedInput().x > 0f)) {
                shouldTopple = false;
            }

            //  checks if should stop toppling
            if(curToppling != shouldTopple) {
                if(curToppling) {
                    tweener.DOKill();
                    tweener.localPosition = Vector3.zero;
                }
                else { 
                    tweener.DOKill();
                    tweener.DOLocalMoveX(1f, toppleTime).OnComplete(() => {
                        toppleEvents.Invoke();
                    }).OnUpdate(() => {
                        var topOff = Vector3.right * toppleRange * tweener.localPosition.x * (toppleRight ? 1f : -1f);
                        transform.position = toppleOrigin + topOff;
                    });
                }
                curToppling = shouldTopple;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
