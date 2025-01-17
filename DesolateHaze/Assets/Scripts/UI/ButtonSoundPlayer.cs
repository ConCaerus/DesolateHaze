using UnityEngine;

public class ButtonSoundPlayer : Singleton<ButtonSoundPlayer> {
    [SerializeField] AudioPoolInfo buttonSound, clickSound;

    public void playButtonSound() {
        AudioManager.I.playButtonSound(buttonSound, 1f);
    }
    public void playClickSound() {
        AudioManager.I.playButtonSound(clickSound, 1f);
    }
}
