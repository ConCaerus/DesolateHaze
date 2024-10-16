using UnityEngine;

public class CraneInstance : MonoBehaviour {
    [SerializeField] Transform body;
    [SerializeField] float speed;
    [SerializeField] bool xAxis = true, yAxis = true;

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
        body.transform.position += new Vector3(xAxis ? savedDir.x : 0f, yAxis ? savedDir.y : 0f, 0f) * speed * 10f * Time.deltaTime;
    }
}
