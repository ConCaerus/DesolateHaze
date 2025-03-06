using System;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInstance : MonoBehaviour {
    public Transform seatPos;
    public Transform endPos;
    public float speed;
    public Rigidbody rb;
    [SerializeField] List<Collider> toggledCols = new List<Collider>();
    [SerializeField] LayerMask inUseExclude, inUseInclude;

    public static Action runOnEnter = () => { }, runOnExit = () => { };

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
