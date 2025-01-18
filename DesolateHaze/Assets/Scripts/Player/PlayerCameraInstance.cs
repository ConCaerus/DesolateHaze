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

    public void checkForHasCamera() {
        //  checks if before camera pickup area
        if(CheckpointManager.I.aType == Saver.areaType.Outside || CheckpointManager.I.aType == Saver.areaType.Onsite) {
            hasCamera = false;
            return;
        }

        //  checks if after camera pickup area
        if(CheckpointManager.I.aType != Saver.areaType.Inside) {
            hasCamera = true;
            return;
        }

        //  checks if before camera pickup pos
        hasCamera = transform.position.x > PlayersCameraPickupable.I.transform.position.x;
        if(hasCamera)
            Destroy(PlayersCameraPickupable.I.gameObject);
    }
}
