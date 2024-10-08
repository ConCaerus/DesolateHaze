using UnityEngine;

public class BusEyeFollow : MonoBehaviour {
    [SerializeField] Transform eyes;
    [SerializeField] float speed;
    [SerializeField] float maxOffset;

    Transform pTrans;

    private void Start() {
        pTrans = PlayerMovement.I.transform;
    }

    private void LateUpdate() {
        followPlayer();
    }

    void followPlayer() {
        Vector2 offset = transform.position - pTrans.position;
        if(offset.magnitude > maxOffset)
            offset = offset.normalized * maxOffset;
        offset.y *= 0.5f;
        eyes.localPosition = offset * speed;
    }
}
