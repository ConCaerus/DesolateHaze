using UnityEngine;

public class PlayerCameraInstance : Singleton<PlayerCameraInstance> {
    bool hc = false;
    public bool hasCamera {
        get {
            return hc;
        }
        set {
            hc = value;
            cam.SetActive(value);
        }
    }
    [SerializeField] GameObject cam;
}
