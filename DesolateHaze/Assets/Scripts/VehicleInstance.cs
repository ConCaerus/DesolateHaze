using System.Collections.Generic;
using UnityEngine;

public class VehicleInstance : MonoBehaviour {
    public Transform seatPos;
    public float speed;
    public Rigidbody rb;
    [SerializeField] List<Collider> toggledCols = new List<Collider>();
    [SerializeField] LayerMask inUseExclude, inUseInclude;
    List<LayerMask> prevExcludes = new List<LayerMask>(), prevIncludes = new List<LayerMask>();

    private void Start() {
        foreach(var i in toggledCols) {
            prevExcludes.Add(i.excludeLayers);
            prevIncludes.Add(i.includeLayers);
        }
    }

    public void toggleDriving() {
        PlayerMovement.I.setCurDriving(PlayerMovement.I.curState == PlayerMovement.pMovementState.Driving ? null : this);
    }

    public void playerEnter() {
        return;
        foreach(var i in toggledCols) {
            i.excludeLayers = inUseExclude;
            i.includeLayers = inUseInclude;
        }
    }
    public void playerExit() {
        return;
        for(int i = 0; i < toggledCols.Count; i++) {
            toggledCols[i].includeLayers = prevIncludes[i];
            toggledCols[i].excludeLayers = prevExcludes[i];
        }
    }
}
