using UnityEngine;

public class DumpsterPushSoundManager : Singleton<DumpsterPushSoundManager> {
    [SerializeField] ASourceInstance asi;
    [SerializeField] AudioClip squeak;

    private void LateUpdate() {
        manageSound();
    }

    void manageSound() {
        var shouldPlay = PlayerMovement.I.curState == PlayerMovement.pMovementState.Pushing && PlayerMovement.I.getCurPushing().transform == transform;
        if(shouldPlay && !asi.isPlaying())
            asi.playSound(squeak, false, false, 1f);
        else if(!shouldPlay && asi.isPlaying())
            asi.stopPlaying();
    }
}
