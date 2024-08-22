using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] float introMuteTime;
    [SerializeField] GameObject sourcePref;
    [SerializeField] int walkPoolCount, groanPoolCount, explosionPoolCount, bouncePoolCount, windowPoolCount, hurtPoolCount, boardPoolCount, corpsePoolCount;

    List<ASourceInstance> walkSources = new List<ASourceInstance>(), groanSources = new List<ASourceInstance>();
    List<ASourceInstance> explosionSources = new List<ASourceInstance>(), bounceSources = new List<ASourceInstance>();
    List<ASourceInstance> windowSources = new List<ASourceInstance>(), hurtSources = new List<ASourceInstance>();
    List<ASourceInstance> boardSources = new List<ASourceInstance>(), corpseSources = new List<ASourceInstance>();

    bool mute = true;

    enum audioType {
        None, Walk, Groan, Explosion, Bounce, Window, Hurt, Board, Corpse
    }

    private void Start() {
        for(int i = 0; i < walkPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            walkSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < groanPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            groanSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < explosionPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            explosionSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < bouncePoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            bounceSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < windowPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            windowSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < hurtPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            hurtSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < boardPoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            boardSources.Add(a.GetComponent<ASourceInstance>());
        }
        for(int i = 0; i < corpsePoolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            corpseSources.Add(a.GetComponent<ASourceInstance>());
        }

        Invoke("unmute", introMuteTime);
    }

    void unmute() {
        mute = false;
    }

    public void playWalkSound(Vector2 point, AudioClip clip) {
        if(walkSources.Count == 0 || mute) return;
        var asi = walkSources[0];
        walkSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Walk));
    }
    public void playGroanSound(Vector2 point, AudioClip clip) {
        if(groanSources.Count == 0 || mute) return;
        var asi = groanSources[0];
        groanSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Groan));
    }
    public void playExplosionSound(Vector2 point, AudioClip clip) {
        if(explosionSources.Count == 0 || mute) return;
        var asi = explosionSources[0];
        explosionSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Explosion));
    }
    public void playBounceSound(Vector2 point, AudioClip clip) {
        if(bounceSources.Count == 0 || mute) return;
        var asi = bounceSources[0];
        bounceSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Bounce));
    }
    public void playWindowSound(Vector2 point, AudioClip clip) {
        if(windowSources.Count == 0 || mute) return;
        var asi = windowSources[0];
        windowSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Window));
    }
    public void playHurtSound(Vector2 point, AudioClip clip) {
        if(hurtSources.Count == 0 || mute) return;
        var asi = hurtSources[0];
        hurtSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Hurt));
    }
    public void playBoardSound(Vector2 point, AudioClip clip) {
        if(boardSources.Count == 0 || mute) return;
        var asi = boardSources[0];
        boardSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Board));
    }
    public void playCorpseSound(Vector2 point, AudioClip clip) {
        if(corpseSources.Count == 0 || mute) return;
        var asi = corpseSources[0];
        corpseSources.RemoveAt(0);
        asi.transform.position = point;
        asi.playSound(clip, false, true);
        StartCoroutine(repoolSource(asi, clip, audioType.Corpse));
    }

    IEnumerator repoolSource(ASourceInstance asi, AudioClip playedClip, audioType type) {
        yield return new WaitForSeconds(playedClip.length);
        switch(type) {
            case audioType.Walk: walkSources.Add(asi); break;
            case audioType.Groan: groanSources.Add(asi); break;
            case audioType.Explosion: explosionSources.Add(asi); break;
            case audioType.Bounce: bounceSources.Add(asi); break;
            case audioType.Window: windowSources.Add(asi); break;
            case audioType.Hurt: hurtSources.Add(asi); break;
            case audioType.Board: boardSources.Add(asi); break;
            case audioType.Corpse: corpseSources.Add(asi); break;
        }
    }
}
