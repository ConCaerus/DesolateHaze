using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager> {
    public Saver.areaType aType;
    public List<CheckpointInstance> checkpoints { get; private set; } = new List<CheckpointInstance>();

    [HideInInspector] public bool initted = false;

    [SerializeField] GameObject playerPref;

    [SerializeField] Color outsideColor, onsiteColor, insideColor, underColor, throughColor, onColor, endColor;

    protected override void awake() {
        if(aType == Saver.areaType.None) return;
        //  stores all available checkpoints (must be children of this)
        foreach(var i in transform.GetComponentsInChildren<CheckpointInstance>()) {
            checkpoints.Add(i);
        }
        StartCoroutine(playerSpawner());
    }

    IEnumerator playerSpawner() {
        Instantiate(playerPref);
        int catcher = 10;
        yield return new WaitForFixedUpdate();
        do {
            yield return new WaitForFixedUpdate();
            catcher--;
        }
        while((PlayerMovement.I == null || !Saver.getLastCheckpoint(this, PlayerMovement.I).triggered) && catcher > 0);
        //  spawns player at the last triggered checkpoint
        var lastCheckpoint = Saver.getLastCheckpoint(this, PlayerMovement.I);
        CameraMovement.I.snapToPosition(PlayerMovement.I.transform.position = lastCheckpoint.pos);
        PlayerMovement.I.rb.linearVelocity = Vector3.zero;
        PlayerMovement.I.speedMod = lastCheckpoint.playerSpeedMod;
        PlayerMovement.I.initted = true;
        //  sets already triggered checkpoints
        foreach(var i in checkpoints) {
            if(Saver.hasTriggeredCheckpoint(this, PlayerMovement.I, i.transform.position))
                i.triggered = true;
        }

        PlayerCameraInstance.I.checkForHasCamera();
    }

    public Color getColor(Saver.areaType aType) {
        switch(aType) {
            case Saver.areaType.Outside: return outsideColor;
            case Saver.areaType.Onsite: return onsiteColor;
            case Saver.areaType.Inside: return insideColor;
            case Saver.areaType.Under: return underColor;
            case Saver.areaType.Through: return throughColor;
            case Saver.areaType.On: return onColor;
            case Saver.areaType.End: return endColor;
            default: return Color.white;
        }
    }
}
