using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASourceInstance : MonoBehaviour {
    [SerializeField] AudioSource source;

    private void OnEnable() {
        OptionsCanvas.settingsChanged += updateSettings;
        StartCoroutine(initWaiter());
    }

    private void OnDisable() {
        OptionsCanvas.settingsChanged -= updateSettings;
    }

    public void updateSettings() {
        source.volume = OptionsCanvas.I.getVolume();
    }

    IEnumerator initWaiter() {
        while(OptionsCanvas.I == null)
            yield return new WaitForEndOfFrame();
        updateSettings();
    }

    public void playSound(AudioClip clip, bool playOnPlayer, bool randomize, float volMod) {
        source.volume = OptionsCanvas.I.getVolume() * volMod;
        source.pitch = randomize ? Random.Range(0.75f, 1.25f) : 1f;

        if(playOnPlayer && PlayerMovement.I != null)
            transform.position = PlayerMovement.I.transform.position + Vector3.up * 1f;
        source.clip = clip;
        source.Play();
    }
    public void modVolume(float volMod) {
        source.volume = OptionsCanvas.I.getVolume() * volMod;
    }
    public float getCurVolumeMod() {
        return source.volume / OptionsCanvas.I.getVolume();
    }
    public void stopPlaying() {
        source.Stop();
    }

    public bool isPlaying() {
        return source.isPlaying;
    }
    public AudioSource getSource() {
        return source;
    }
}
