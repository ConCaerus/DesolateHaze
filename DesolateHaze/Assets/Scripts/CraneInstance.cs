using UnityEngine;
using UnityEngine.Events;

public class CraneInstance : MonoBehaviour {
    [SerializeField] Transform body;
    [SerializeField] float speed, accSpeed;
    [SerializeField] bool xAxis = true, yAxis = true;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform button;

    [SerializeField] bool rotationalMovement = false;
    [SerializeField] Transform rotPoint;
    [SerializeField] float rotSpeed;
    [SerializeField] BoxCollider kill;

    [SerializeField] bool hasLowerEvent = false;
    [SerializeField] float lowerEventY;
    [SerializeField] UnityEvent lowerEvent;

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
        if(kill != null)
            kill.enabled = false;
    }

    private void Update() {
        if(active || rb.linearVelocity.magnitude > 0f)
            move();
        if(hasLowerEvent) checkForLowerEvent();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void trigger() {
        active = true;
        rb.isKinematic = false;
        if(kill != null)
            kill.enabled = true;
    }
    public void detrigger() {
        active = false;
        rb.isKinematic = true;
        if(kill != null)
            kill.enabled = false;
    }
    public void uninteract() {
        PlayerInteraction.I.setCurInteractable(null);
    }

    void updateDir(Vector2 dir) {
        if(active)
            savedDir = dir;
    }

    void move() {
        var target = !active ? Vector3.zero : new Vector3(!rotationalMovement && xAxis ? savedDir.x : 0f, yAxis ? savedDir.y : 0f, 0f) * speed * 100f * Time.deltaTime;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target, accSpeed * 100f * Time.deltaTime);

        if(rotationalMovement) {
            var rotTarget = Vector3.forward * savedDir.x * rotSpeed * 10f * Time.deltaTime;
            rotPoint.localEulerAngles += rotTarget;
        }
    }

    void checkForLowerEvent() {
        if(rb.transform.localPosition.z <= lowerEventY) {
            hasLowerEvent = false;
            lowerEvent.Invoke();
        }
    }
}
