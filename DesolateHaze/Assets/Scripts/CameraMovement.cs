using UnityEngine;

public class CameraMovement : Singleton<CameraMovement> {
    [SerializeField] float speed;
    [SerializeField] float yOffset;

    Transform playerTrans;

    [HideInInspector] public bool canMove = true;

    private void LateUpdate() {
        if(canMove)
            followPlayer();
    }

    void followPlayer() {
        if(playerTrans == null) {
            if(PlayerMovement.I == null) return;
            playerTrans = PlayerMovement.I.transform;
        }
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerTrans.position.x, playerTrans.position.y + yOffset, transform.position.z), speed * Time.deltaTime);
    }

    public void snapToPosition(Vector3 pos) {
        transform.position = new Vector3(pos.x, pos.y + yOffset, transform.position.z);
    }
}
