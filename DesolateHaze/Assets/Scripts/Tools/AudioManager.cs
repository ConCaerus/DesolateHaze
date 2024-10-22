using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    [SerializeField] float introMuteTime;
    [SerializeField] GameObject sourcePref;
    [SerializeField] List<AudioPoolInfo> poolInfo = new List<AudioPoolInfo>();

    List<List<ASourceInstance>> poolSources = new List<List<ASourceInstance>>();

    bool mute = true;

    private void Start() {
        for(int p = 0; p < poolInfo.Count; p++) {
            poolSources.Add(new List<ASourceInstance>());
            for(int i = 0; i < poolInfo[p].poolCount; i++) {
                var a = Instantiate(sourcePref, transform);
                poolSources[p].Add(a.GetComponent<ASourceInstance>());
            }
        }

        Invoke("unmute", introMuteTime);
    }

    void unmute() {
        mute = false;
    }

    public void playSound(Vector3 point, int poolIndex, float volMod) {
        if(poolIndex < 0 || poolIndex >= poolInfo.Count) return;
        if(poolSources[poolIndex].Count == 0 || mute) return;
        var asi = poolSources[poolIndex][0];
        poolSources[poolIndex].RemoveAt(0);
        asi.transform.position = point;
        var clip = poolInfo[poolIndex].clips[Random.Range(0, poolInfo[poolIndex].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, poolIndex, null));
    }
    public void playFollowingSound(Transform trans, int poolIndex, float volMod) {
        if(poolIndex < 0 || poolIndex >= poolInfo.Count) return;
        if(poolSources[poolIndex].Count == 0 || mute) return;
        var asi = poolSources[poolIndex][0];
        poolSources[poolIndex].RemoveAt(0);
        var clip = poolInfo[poolIndex].clips[Random.Range(0, poolInfo[poolIndex].clips.Count)];
        asi.playSound(clip, false, true, volMod);
        StartCoroutine(repoolSource(asi, clip.length, poolIndex, trans));
    }

    IEnumerator repoolSource(ASourceInstance asi, float length, int poolIndex, Transform followTrans) {
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
        poolSources[poolIndex].Add(asi);
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
    public List<AudioClip> clips = new List<AudioClip>();
}
