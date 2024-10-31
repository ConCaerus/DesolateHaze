using UnityEngine;

public class TreeSeesawSoundPlayer : MonoBehaviour {
    [SerializeField] ASourceInstance asi;
    [SerializeField] AudioClip creak;
    [SerializeField] Rigidbody rb;

    private void FixedUpdate() {
        if(rb.angularVelocity.z != 0f && !asi.isPlaying() && gameObject.activeInHierarchy)
            asi.playSound(creak, false, false, 1f);
    }
}
