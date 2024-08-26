using UnityEngine;

public class CheckpointInstance : MonoBehaviour {
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
        triggered = true;
        Saver.triggerCheckpoint(CheckpointManager.I, transform.position);
    }
}
