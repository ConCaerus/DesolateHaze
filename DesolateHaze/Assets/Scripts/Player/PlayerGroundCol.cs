using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCol : MonoBehaviour {
    CapsuleCollider c;
    float fullSize;

    Transform tweener;

    private void Start() {
        tweener = new GameObject("Tweener").transform;
        tweener.parent = transform;
        tweener.localPosition = Vector3.zero;
        c = GetComponent<CapsuleCollider>();
        fullSize = c.radius;
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.touchedGround(col);
        tweener.DOKill();
        tweener.DOLocalMoveY(fullSize, .15f).OnUpdate(() => { c.radius = tweener.localPosition.y; });
    }
    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
        PlayerMovement.I.leftGround(col);
        tweener.DOKill();
        tweener.DOLocalMoveY(fullSize / 2f, .15f).OnUpdate(() => { c.radius = tweener.localPosition.y; });
    }
    private void OnTriggerStay(Collider col) {
        if(!PlayerMovement.I.grounded) {
            if(col.gameObject.tag != "Ground" && col.gameObject.tag != "Box") return;
            PlayerMovement.I.touchedGround(col);
            tweener.DOKill();
            tweener.DOLocalMoveY(fullSize, .15f).OnUpdate(() => { c.radius = tweener.localPosition.y; });
        }
    }
}
