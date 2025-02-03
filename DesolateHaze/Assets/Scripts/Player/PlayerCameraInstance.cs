using DG.Tweening;
using System.Collections;
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
    [SerializeField] GameObject polaroidPref;
    [SerializeField] Transform polaroidOrigin, polaroidTarget;
    [SerializeField] float cooldown;

    InputMaster controls;

    Coroutine cooldowner = null;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Camera.performed += ctx => takePicture();
    }

    private void OnDisable() {
        controls.Disable();
    }

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

    void takePicture() {
        if(!hasCamera || cooldowner != null) return;
        var p = Instantiate(polaroidPref, transform);
        p.transform.position = polaroidOrigin.transform.position;
        p.transform.DOLocalMove(polaroidTarget.localPosition, 1f).OnComplete(() => {
            p.transform.parent = null;
            p.GetComponent<Rigidbody>().isKinematic = false;
            p.GetComponent<PolaroidInstance>().init();
        });
        cooldowner = StartCoroutine(cooldownWaiter());

        CameraFlashInstance.I.flash();
    }

    IEnumerator cooldownWaiter() {
        yield return new WaitForSeconds(cooldown);
        cooldowner = null;
    }
}
