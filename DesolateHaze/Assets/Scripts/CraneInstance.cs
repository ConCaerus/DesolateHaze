using UnityEngine;

public class CraneInstance : MonoBehaviour {
    [SerializeField] Transform body;
    [SerializeField] float speed, accSpeed;
    [SerializeField] bool xAxis = true, yAxis = true;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform button;

    bool active = false;

    Vector2 savedDir = Vector2.zero;

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => updateDir(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => updateDir(Vector2.zero);

        button.transform.parent = transform.parent;
        rb.isKinematic = true;
    }

    private void Update() {
        if(active || rb.linearVelocity.magnitude > 0f)
            move();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void trigger() {
        active = true;
        rb.isKinematic = false;
    }
    public void detrigger() {
        active = false;
        rb.isKinematic = true;
    }

    void updateDir(Vector2 dir) {
        if(active)
            savedDir = dir;
    }

    void move() {
        var target = !active ? Vector3.zero : new Vector3(xAxis ? savedDir.x : 0f, yAxis ? savedDir.y : 0f, 0f) * speed * 100f * Time.deltaTime;
        rb.linearVelocity =  Vector3.Lerp(rb.linearVelocity, target, accSpeed * 100f * Time.deltaTime);
    }
}
