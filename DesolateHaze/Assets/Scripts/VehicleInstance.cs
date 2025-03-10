using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInstance : MonoBehaviour {
    public Transform seatPos;
    public Transform endPos;
    [SerializeField] Transform rotTrans;
    public float speed;
    public Rigidbody rb;
    [SerializeField] List<Collider> toggledCols = new List<Collider>();
    [SerializeField] LayerMask inUseExclude, inUseInclude;

    bool fr = true;
    public bool facingRight {
        get { return fr; }
        set {
            fr = value;
            rotTrans.rotation = Quaternion.Euler(0f, fr ? 0f : 180f, 0f);
        }
    }

    public static System.Action runOnEnter = () => { }, runOnExit = () => { };

    private void OnDisable() {
        runOnEnter = () => { };
        runOnExit = () => { };
    }

    public void toggleDriving() {
        PlayerMovement.I.setCurDriving(PlayerMovement.I.curState == PlayerMovement.pMovementState.Driving ? null : this);
    }

    public void playerEnter() {
        runOnEnter();
    }
    public void playerExit() {
        runOnExit();
    }
}
