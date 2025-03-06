using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : Singleton<PlayerAudioManager> {
    [SerializeField] List<walkSoudInfo> walkSounds = new List<walkSoudInfo>();
    Dictionary<groundType, AudioPoolInfo> dic = new Dictionary<groundType, AudioPoolInfo>();

    AudioPoolInfo curWalkSound;

    [System.Serializable]
    struct walkSoudInfo {
        public AudioPoolInfo sound;
        public groundType type;
    }

    Coroutine walker = null;

    [System.Serializable]
    public enum groundType {
        None, Dirt, Concrete, Tile
    }

    private void Start() {
        dic.Clear();
        foreach(var i in walkSounds) {
            dic.Add(i.type, i.sound);
            AudioManager.I.initSound(i.sound);
        }
    }

    public void startWalking(float timeBtwSteps) {
        if(walker == null)
            walker = StartCoroutine(walkerWaiter(timeBtwSteps));
    }
    public void stopWalking() {
        if(walker != null) {
            StopCoroutine(walker);
            walker = null;
        }
    }

    public void setGroundType(groundType t) {
        if(t == groundType.None) return;
        if(!dic.ContainsKey(t)) {
            Debug.LogError("PLAYER AUDIO MANGAER DOES NOT CONTAIN A REFERENCE FOR GROUND TYPE: " + t.ToString());
            return;
        }
        curWalkSound = dic[t];
    }

    IEnumerator walkerWaiter(float timeBtwSteps) {
        while(true) {
            if(PlayerMovement.I.grounded)
                AudioManager.I.playSound(curWalkSound, transform.position, 1f);
            yield return new WaitForSeconds(timeBtwSteps);
        }
    }
}
