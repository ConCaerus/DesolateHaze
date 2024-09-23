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
        var offset = transform.position.x - pTrans.position.x;
        var target = Mathf.Clamp(offset * speed, -maxOffset, maxOffset);
        eyes.localPosition = Vector3.right * target;
    }
}
