using UnityEngine;

public class PressurePlatePlate : MonoBehaviour {
    [SerializeField] PressurePlateInstance instance;

    private void OnCollisionStay(Collision col) {
        instance.heldDown = true;
    }

    private void OnCollisionExit(Collision col) {
        instance.heldDown = false;
    }
}
