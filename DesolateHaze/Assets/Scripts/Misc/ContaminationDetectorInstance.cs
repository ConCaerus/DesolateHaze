using UnityEngine;

public class ContaminationDetectorInstance : MonoBehaviour {
    [SerializeField] GameObject green, red;
    [SerializeField] Collider blocker;

    private void Start() {
        resetEffect();
    }

    public void check() {
        bool pass = !PlayerMovement.I.isContaminated;
        green.SetActive(pass);
        red.SetActive(!pass);
        blocker.enabled = !pass;
    }

    public void resetEffect() {
        green.SetActive(false);
        red.SetActive(false);
    }
}
