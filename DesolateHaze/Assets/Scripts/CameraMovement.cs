using UnityEngine;

public class CameraMovement : Singleton<CameraMovement> {
    [SerializeField] float speed;
    [SerializeField] float yOffset;

    [HideInInspector] public bool canMove = true;

    private void LateUpdate() {
        if(canMove)
            followPlayer();
    }

    void followPlayer() {
        var target = PlayerMovement.I.getCamFollowPos() + Vector3.up * yOffset;
        target.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
    }

    public void snapToPosition(Vector3 pos) {
        transform.position = new Vector3(pos.x, pos.y + yOffset, transform.position.z);
    }
}
