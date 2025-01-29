using UnityEngine;

public class CameraFlashInstance : Singleton<CameraFlashInstance> {
    [SerializeField] GameObject flashObj;
    [SerializeField] float duration;

    private void Start() {
        flashObj.SetActive(false);
    }

    public void flash() {
        flashObj.SetActive(true);
        Invoke("endFlash", duration);
    }

    void endFlash() {
        flashObj.SetActive(false);
    }
}
