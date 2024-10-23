using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] float introMuteTime;
    [SerializeField] GameObject sourcePref;
    [SerializeField] List<AudioPoolInfo> poolInfo = new List<AudioPoolInfo>();
    Dictionary<audioTitle, AudioPoolInfo> dicPoolInfo = new Dictionary<audioTitle, AudioPoolInfo>();

    Dictionary<audioTitle, List<ASourceInstance>> poolSources = new Dictionary<audioTitle, List<ASourceInstance>>();

    [System.Serializable]
    public enum audioTitle {
        None, Explosion
    }

    bool mute = true;

    private void Start() {
        foreach(var i in poolInfo) {
            dicPoolInfo.Add(i.title, i);
        }
        for(int p = 0; p < poolInfo.Count; p++) {
            poolSources.Add(poolInfo[p].title, new List<ASourceInstance>());
            for(int i = 0; i < poolInfo[p].poolCount; i++) {
                var a = Instantiate(sourcePref, transform);
                poolSources[poolInfo[p].title].Add(a.GetComponent<ASourceInstance>());
            }
        }

        Invoke("unmute", introMuteTime);
    }

    void unmute() {
        mute = false;
    }

    public void playSound(Vector3 point, audioTitle title, float volMod) {
        if(title == audioTitle.None) return;
        if(poolSources[title].Count == 0 || mute) return;
        var asi = poolSources[title][0];
        poolSources[title].RemoveAt(0);
        asi.transform.position = point;
        var clip = dicPoolInfo[title].clips[Random.Range(0, dicPoolInfo[title].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, title, null));
    }
    public void playFollowingSound(Transform trans, audioTitle title, float volMod) {
        if(title == audioTitle.None) return;
        if(poolSources[title].Count == 0 || mute) return;
        var asi = poolSources[title][0];
        poolSources[title].RemoveAt(0);
        var clip = dicPoolInfo[title].clips[Random.Range(0, dicPoolInfo[title].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, title, trans));
    }

    IEnumerator repoolSource(ASourceInstance asi, float length, audioTitle title, Transform followTrans) {
        if(followTrans == null)
            yield return new WaitForSeconds(length);
        else {
            var startTime = Time.time;
            while(length > 0f && followTrans != null) {
                asi.transform.position = followTrans.position;
                yield return new WaitForEndOfFrame();
                length -= Time.time - startTime;
                startTime = Time.time;
            }
        }
        poolSources[title].Add(asi);
    }

    public AudioClip getClip(int poolIndex) {
        if(poolIndex >= poolInfo.Count) return null;
        return poolInfo[poolIndex].clips[0];
    }
    public Vector3 getListenerPos() {
        return FindObjectOfType<AudioListener>().transform.position;
    }
}

[System.Serializable] 
public class AudioPoolInfo {
    public int poolCount;
    public AudioManager.audioTitle title;
    public List<AudioClip> clips = new List<AudioClip>();
}
