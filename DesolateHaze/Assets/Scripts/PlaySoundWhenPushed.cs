using UnityEngine;

public class PlaySoundWhenPushed : MonoBehaviour {
    [SerializeField] ASourceInstance asi;
    [SerializeField] AudioClip sound;

    private void FixedUpdate() {
        bool shouldPlay = PlayerMovement.I.curState == PlayerMovement.pMovementState.Pushing && PlayerMovement.I.getCurPushing().transform == transform;
        if(asi.isPlaying() && !shouldPlay) asi.stopPlaying();
        else if(!asi.isPlaying() && shouldPlay) asi.playSound(sound, false, false, 1f);
    }
}
