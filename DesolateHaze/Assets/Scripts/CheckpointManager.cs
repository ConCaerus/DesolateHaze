using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager> {
    public Saver.areaType aType;
    public List<CheckpointInstance> checkpoints { get; private set; } = new List<CheckpointInstance>();

    [HideInInspector] public bool initted = false;

    [SerializeField] GameObject playerPref;

    protected override void awake() {
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
        PlayerMovement.I.speedMod = lastCheckpoint.playerSpeedMod;

        //  sets already triggered checkpoints
        foreach(var i in checkpoints) {
            if(Saver.hasTriggeredCheckpoint(this, PlayerMovement.I, i.transform.position))
                i.triggered = true;
        }
    }
}
