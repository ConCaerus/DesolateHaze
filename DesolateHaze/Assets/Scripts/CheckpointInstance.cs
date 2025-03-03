using UnityEngine;

public class CheckpointInstance : MonoBehaviour {
    public PlayerAudioManager.groundType groundType;

    bool t = false;
    public bool triggered {
        get { return t; }
        set {
            t = value;
            if(t) {
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag != "Player") return;
        triggered = true;
        Saver.triggerCheckpoint(CheckpointManager.I, PlayerMovement.I, transform.position);
    }
}
