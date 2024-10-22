using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer> {
    [SerializeField] AudioClip ambience, music;
    [SerializeField] ASourceInstance ambiencePlayer, musicPlayer;
    [SerializeField] float minTimeBtw, maxTimeBtw;

    Transform tweener;

    private void Start() {
        DOTween.Init();
        tweener = new GameObject().transform;
        tweener.parent = transform;
        tweener.localPosition = Vector3.zero;
        if(ambience != null)
            ambiencePlayer.playSound(ambience, false, false, 1f);
        StartCoroutine(musicLoop());
    }

    IEnumerator musicLoop() {
        while(true) {
            float loopWaitTime = Random.Range(minTimeBtw, maxTimeBtw);
            yield return new WaitForSeconds(loopWaitTime);
            musicPlayer.playSound(music, false, false, 0f);
            tweener.DOKill();
            tweener.localPosition = Vector3.zero;
            tweener.DOLocalMoveX(1f, 1f).OnUpdate(() => {
                ambiencePlayer.modVolume(1f - (tweener.localPosition.x / 1f));
                musicPlayer.modVolume(tweener.localPosition.x / 1f);
            });
            yield return new WaitForSeconds(1f);
            ambiencePlayer.modVolume(0f);
            musicPlayer.modVolume(1f);
            yield return new WaitForSeconds(music.length - 2f);
            tweener.DOKill();
            tweener.localPosition = Vector3.zero;
            tweener.DOLocalMoveX(1f, 1f).OnUpdate(() => {
                ambiencePlayer.modVolume(tweener.localPosition.x / 1f);
                musicPlayer.modVolume(1f - (tweener.localPosition.x / 1f));
            });
            yield return new WaitForSeconds(1f);
            ambiencePlayer.modVolume(1f);
            musicPlayer.modVolume(0f);
        }
    }

    public bool isPlayingMusic() {
        return musicPlayer.isPlaying();
    }
}
