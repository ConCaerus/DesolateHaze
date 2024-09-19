using UnityEngine;

public class DestroyerInstance : MonoBehaviour {

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Player")
            TransitionCanvas.I.loadGameAfterDeath(0f);
        else Destroy(col.gameObject);
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player")
            TransitionCanvas.I.loadGameAfterDeath(0f);
        else Destroy(col.gameObject);
    }
}
