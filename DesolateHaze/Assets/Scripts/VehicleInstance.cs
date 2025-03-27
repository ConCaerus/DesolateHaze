using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInstance : MonoBehaviour {
    public Transform seatPos;
    public Transform endPos;
    [SerializeField] Transform rotTrans;
    public float speed;
    public Rigidbody rb;
    [SerializeField] LayerMask inUseExclude, inUseInclude;
    public bool facingRight = true;

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
        PlayerMovement.I.rb.isKinematic = true;
        PlayerMovement.I.rb.linearVelocity = Vector3.zero;
        PlayerMovement.I.rb.interpolation = RigidbodyInterpolation.None;
    }
    public void playerExit() {
        runOnExit();
        PlayerMovement.I.rb.isKinematic = false;
        PlayerMovement.I.rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
