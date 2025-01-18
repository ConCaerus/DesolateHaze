using UnityEngine;

public class PlayersCameraPickupable : Singleton<PlayersCameraPickupable> {

    public void pickup() {
        PlayerCameraInstance.I.hasCamera = true;
        Destroy(gameObject);
    }
}
