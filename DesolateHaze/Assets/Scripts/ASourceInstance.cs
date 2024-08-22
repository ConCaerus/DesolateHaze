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

    public void playSound(AudioClip clip, bool playOnPlayer, bool randomize) {
        source.pitch = randomize ? Random.Range(0.75f, 1.25f) : 0.8f;

        if(playOnPlayer && PlayerMovement.I != null)
            transform.position = (Vector2)PlayerMovement.I.transform.position + Vector2.up * 1f;
        source.PlayOneShot(clip);
    }
    public void stopPlaying() {
        source.Stop();
    }
}
