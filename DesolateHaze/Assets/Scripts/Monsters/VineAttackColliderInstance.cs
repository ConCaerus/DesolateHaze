using UnityEngine;

public class VineAttackColliderInstance : MonoBehaviour {
    [SerializeField] VineInstance vInstance;
    [SerializeField] MeatMoundInstance mInstance;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            if(vInstance != null)
                vInstance.killPlayer();
            if(mInstance != null)
                mInstance.killPlayer();
        }
    }
}
