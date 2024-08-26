using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager> {
    public Saver.areaType aType;
    public List<CheckpointInstance> checkpoints { get; private set; } = new List<CheckpointInstance>();

    [SerializeField] GameObject playerPref;

    protected override void awake() {
        //  stores all available checkpoints (must be children of this)
        foreach(var i in transform.GetComponentsInChildren<CheckpointInstance>()) {
            checkpoints.Add(i);
        }

        //  sets already triggered checkpoints
        foreach(var i in checkpoints) {
            if(Saver.hasTriggeredCheckpoint(this, i.transform.position))
                i.triggered = true;
        }

        StartCoroutine(playerSpawner());
    }

    IEnumerator playerSpawner() {
        yield return new WaitForFixedUpdate();
        //  spawns player at the last triggered checkpoint
        CameraMovement.I.snapToPosition(Instantiate(playerPref).transform.position = Saver.getLastCheckpointPos(this));
    }
}
