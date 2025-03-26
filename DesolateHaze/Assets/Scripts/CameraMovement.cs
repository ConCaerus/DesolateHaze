using DG.Tweening;
using UnityEngine;

public class CameraMovement : Singleton<CameraMovement> {
    [SerializeField] float speed;
    [SerializeField] float yOffset;

    [HideInInspector] public bool canMove = true;

    Vector3 dynamicOffset = Vector3.zero;

    bool isAnchored = false;
    Vector2 anchorPoint;
    float normFOV;

    private void Start() {
        normFOV = Camera.main.fieldOfView;
    }

    private void LateUpdate() {
        if(!canMove) return;
        if(isAnchored)
            followAnchor();
        else if(!PlayerMovement.I.isDead)
            followPlayer();
    }

    void followPlayer() {
        var target = PlayerMovement.I.getCamFollowPos() + Vector3.up * yOffset;
        target.z = transform.position.z;
        target += dynamicOffset;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    }
    void followAnchor() {
        Vector3 target = anchorPoint + Vector2.up;
        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, speed * .5f * Time.deltaTime);
    }

    public void snapToPosition(Vector3 pos) {
        transform.position = new Vector3(pos.x, pos.y + yOffset, transform.position.z);
    }

    //  (x, y) - anchorPoint, z - fov
    public void setAnchorPoint(Vector2 point, float fov) {
        Camera.main.DOKill();
        isAnchored = true;
        Camera.main.DOFieldOfView(fov, 1f);
        anchorPoint = point;
    }
    public void unAnchorPoint() {
        Camera.main.DOKill();
        isAnchored = false;
        anchorPoint = Vector3.zero;
        Camera.main.DOFieldOfView(normFOV, 1f);
    }

    public void setDynamicOffset(Vector3 o) {
        dynamicOffset = o;
    }
}