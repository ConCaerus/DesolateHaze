using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] float introMuteTime;
    [SerializeField] GameObject sourcePref;
    Dictionary<string, AudioPoolInfo> poolInfo = new Dictionary<string, AudioPoolInfo>();

    Dictionary<string, List<ASourceInstance>> poolSources = new Dictionary<string, List<ASourceInstance>>();

    bool mute = true;

    private void Start() {
        Invoke("unmute", introMuteTime);
    }

    void unmute() {
        mute = false;
    }

    public void playSound(AudioPoolInfo info, Vector3 point, float volMod) {
        if(info == null || string.IsNullOrEmpty(info.title)) return;
        if(!poolInfo.ContainsKey(info.title)) initSound(info);
        if(poolSources[info.title].Count == 0 || mute) return;
        var asi = poolSources[info.title][0];
        poolSources[info.title].RemoveAt(0);
        asi.transform.position = point;
        var clip = poolInfo[info.title].clips[Random.Range(0, poolInfo[info.title].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, info.title, null));
    }
    public void playFollowSound(AudioPoolInfo info, Transform point, float volMod) {
        if(string.IsNullOrEmpty(info.title)) return;
        if(!poolInfo.ContainsKey(info.title)) initSound(info);
        if(poolSources[info.title].Count == 0 || mute) return;
        var asi = poolSources[info.title][0];
        poolSources[info.title].RemoveAt(0);
        var clip = poolInfo[info.title].clips[Random.Range(0, poolInfo[info.title].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, info.title, point));
    }
    public void playButtonSound(AudioPoolInfo info, float volMod) {
        if(string.IsNullOrEmpty(info.title)) return;
        if(!poolInfo.ContainsKey(info.title)) initSound(info);
        if(poolSources[info.title].Count == 0 || mute) return;
        var asi = poolSources[info.title][0];
        poolSources[info.title].RemoveAt(0);
        var clip = poolInfo[info.title].clips[Random.Range(0, poolInfo[info.title].clips.Count)];
        asi.playSound(clip, true, false, volMod);
        StartCoroutine(repoolSource(asi, clip.length, info.title, null));
    }

    public void initSound(AudioPoolInfo info) {
        if(poolInfo.ContainsKey(info.title)) return;
        poolInfo.Add(info.title, info);
        poolSources.Add(info.title, new List<ASourceInstance>());
        for(int i = 0; i < info.poolCount; i++) {
            var a = Instantiate(sourcePref, transform);
            poolSources[info.title].Add(a.GetComponent<ASourceInstance>());
        }
    }

    IEnumerator repoolSource(ASourceInstance asi, float length, string title, Transform followTrans) {
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

    public AudioClip getClip(string title) {
        if(string.IsNullOrEmpty(title)) return null;
        return poolInfo[title].clips[0];
    }
    public Vector3 getListenerPos() {
        return FindFirstObjectByType<AudioListener>().transform.position;
    }
}

[System.Serializable]
public class AudioPoolInfo {
    public string title;
    public int poolCount;
    public List<AudioClip> clips = new List<AudioClip>();

    public AudioPoolInfo(string t, int pc, List<AudioClip> cs) {
        title = t;
        poolCount = pc;
        clips = cs;
    }
}
