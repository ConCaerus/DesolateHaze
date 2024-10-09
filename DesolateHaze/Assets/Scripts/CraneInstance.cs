using UnityEngine;

public class CraneInstance : MonoBehaviour {
    [SerializeField] Transform p1, p2;
    [SerializeField] float speed;

    bool active = false;

    Vector2 savedDir = Vector2.zero;

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => updateDir(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => updateDir(Vector2.zero);
    }

    private void Update() {
        if(active)
            move();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void trigger() {
        active = true;
    }
    public void detrigger() {
        active = false;
    }

    void updateDir(Vector2 dir) {
        if(active)
            savedDir = dir;
    }

    void move() {
        if(savedDir.x > 0) {
            p1.localEulerAngles += Vector3.forward * speed * 10f * Time.deltaTime;
            p2.localEulerAngles += Vector3.forward * speed * 10f * Time.deltaTime;
        }
        else if(savedDir.x < 0) {
            p1.localEulerAngles -= Vector3.forward * speed * 10f * Time.deltaTime;
            p2.localEulerAngles -= Vector3.forward * speed * 10f * Time.deltaTime;
        }
    }
}
