using UnityEngine;

public class VineAttackColliderInstance : MonoBehaviour {
    [SerializeField] VineInstance vInstance;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            vInstance.killPlayer();
        }
    }
}
