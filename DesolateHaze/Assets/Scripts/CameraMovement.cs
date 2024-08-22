using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] float speed;
    [SerializeField] float yOffset;

    Transform playerTrans;

    [HideInInspector] public bool canMove = true;

    private void Start() {
        playerTrans = PlayerMovement.I.transform;
    }

    private void LateUpdate() {
        if(canMove)
            followPlayer();
    }

    void followPlayer() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerTrans.position.x, playerTrans.position.y + yOffset, transform.position.z), speed * Time.deltaTime);
    }
}
