using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer> {
    [SerializeField] AudioClip ambience;
    [SerializeField] ASourceInstance ambiencePlayer;

    private void Start() {
        if(ambience != null) {
            StartCoroutine(ambienceLoop());
        }
    }

    IEnumerator ambienceLoop() {
        while(true) {
            ambiencePlayer.playSound(ambience, false, false, 1f);
            yield return new WaitForSeconds(ambience.length);
        }
    }
}
