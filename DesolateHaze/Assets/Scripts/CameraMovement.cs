using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : Singleton<CameraMovement> {
    [SerializeField] float speed;
    [SerializeField] float yOffset;
    [SerializeField] float maxOffsetFromPlayer; //  used for dynamic offsets and lookat trans

    [HideInInspector] public bool canMove = true;

    [HideInInspector] public Vector3 dynamicOffset = Vector3.zero;
    [HideInInspector] public Transform lookAtTrans = null;

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
        if(lookAtTrans != null) {
            if(Vector2.Distance(PlayerMovement.I.transform.position, lookAtTrans.position) <= maxOffsetFromPlayer) {
                target += lookAtTrans.position;
                target /= 2f;
            }
        }
        target.z = transform.position.z;
        target += dynamicOffset;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    }
    void followAnchor() {
        //  checks if too far away from player
        if(Vector2.Distance(anchorPoint, PlayerMovement.I.transform.position) > maxOffsetFromPlayer) {
            unAnchorPoint();
            return;
        }
        Vector3 target = anchorPoint;
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
}