using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Saver {
    static string playerTag = "PlayerSaveData";
    public enum areaType {
        None, Outside, Onsite, Inside, Under, Through, On, End
    }

    public static ExtractedPlayerSaveData getSave() {
        var data = SaveData.getString(playerTag);
        return (string.IsNullOrEmpty(data) ? new PlayerSaveData() : JsonUtility.FromJson<PlayerSaveData>(data)).extractData();
    }
    public static void storeSave(ExtractedPlayerSaveData saveData) {
        var data = JsonUtility.ToJson(saveData.compressData()); ;
        SaveData.setString(playerTag, data);
    }
    public static void clearSave() {
        SaveData.setString(playerTag, "");
    }

    public static void triggerCheckpoint(CheckpointManager cm, PlayerMovement pm, Vector3 checkPointPos) {
        var save = getSave();
        initCheck(save, cm, pm);

        foreach(var i in save.data[(int)cm.aType - 1]) {
            if(i.pos == checkPointPos) {
                i.triggered = true;
                i.playerSpeedMod = pm.speedMod;
                break;
            }
        }

        storeSave(save);
    }
    public static void untriggerLastCheckpoint(CheckpointManager cm, PlayerMovement pm) {
        var save = getSave();

        var pos = getLastCheckpoint(cm, pm).pos;

        for(int i = 0; i < save.data[(int)cm.aType - 1].Count; i++) {
            if(save.data[(int)cm.aType - 1][i].pos == pos) {
                if(i > 0) save.data[(int)cm.aType - 1][i].triggered = false;
                break;
            }
        }

        storeSave(save);
    }

    public static CheckpointSaveData getLastCheckpoint(CheckpointManager cm, PlayerMovement pm) {
        var save = getSave();
        if(initCheck(save, cm, pm)) {
            for(int i = save.data[(int)cm.aType - 1].Count - 1; i >= 0; i--) {
                if(save.data[(int)cm.aType - 1][i].triggered) {
                    return save.data[(int)cm.aType - 1][i];
                }
            }
        }
        var temp = new CheckpointSaveData(cm.checkpoints[0], 1f);
        temp.triggered = true;
        return temp;
    }
    public static bool hasTriggeredCheckpoint(CheckpointManager cm, PlayerMovement pm, Vector3 pos) {
        if(cm.aType == areaType.None) {
            Debug.LogError("Set the area type of the checkpoint manager");
            return false;
        }
        var save = getSave();

        if(!initCheck(save, cm, pm)) return false;   //  checks for valid save

        //  if old save, check for seen checkpoints
        foreach(var i in save.data[(int)cm.aType - 1]) {
            if(i.pos == pos) {
                return i.triggered;
            }
        }
        return false;
    }

    //  returns false if save not setup, true if has save data
    static bool initCheck(ExtractedPlayerSaveData save, CheckpointManager cm, PlayerMovement pm) {
        if(cm.initted) return true;
        
        //  resets position on all checkpoints
        for(int i = 0; i < save.data[(int)cm.aType - 1].Count; i++) {
            save.data[(int)cm.aType - 1][i].pos = cm.checkpoints[i].transform.position;
        }

        //  checks for newly created checkpoints
        if(save.data[(int)cm.aType - 1].Count != cm.checkpoints.Count) {
            for(int i = save.data[(int)cm.aType - 1].Count; i < cm.checkpoints.Count; i++) {
                var csd = new CheckpointSaveData(cm.checkpoints[i].transform.position, pm.speedMod, false);
                save.data[(int)cm.aType - 1].Add(csd);
            }
        }
        storeSave(save);
        cm.initted = true;
        return false;
    }
}

//  used to send to player prefs (player prefs doesn't like nested lists)
[System.Serializable]
public class PlayerSaveData {
    public List<CheckpointSaveData> outsideData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> onsiteData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> insideData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> underData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> throughData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> onData = new List<CheckpointSaveData>();
    public List<CheckpointSaveData> endData = new List<CheckpointSaveData>();

    public ExtractedPlayerSaveData extractData() {
        var temp = new ExtractedPlayerSaveData();
        temp.data[0].AddRange(outsideData);
        temp.data[1].AddRange(onsiteData);
        temp.data[2].AddRange(insideData);
        temp.data[3].AddRange(underData);
        temp.data[4].AddRange(throughData);
        temp.data[5].AddRange(onData);
        temp.data[6].AddRange(endData);
        return temp;
    }
}

//  used in code (easier to code with nested lists)
[System.Serializable]
public class ExtractedPlayerSaveData {
    public List<List<CheckpointSaveData>> data = new List<List<CheckpointSaveData>>();

    public ExtractedPlayerSaveData() {
        for(int i = 0; i < 7; i++)
            data.Add(new List<CheckpointSaveData>());
    }

    public PlayerSaveData compressData() {
        var temp = new PlayerSaveData();
        temp.outsideData.AddRange(data[0]);
        temp.onsiteData.AddRange(data[1]);
        temp.insideData.AddRange(data[2]);
        temp.underData.AddRange(data[3]);
        temp.throughData.AddRange(data[4]);
        temp.onData.AddRange(data[5]);
        temp.endData.AddRange(data[6]);
        return temp;
    }
}

[System.Serializable]
public class CheckpointSaveData {
    public Vector3 pos;
    public float playerSpeedMod;
    public bool triggered = false;

    public CheckpointSaveData(Vector3 p, float sm, bool t) {
        pos = p;
        playerSpeedMod = sm;
        triggered = t;
    }
    public CheckpointSaveData(CheckpointInstance ci, float sm) {
        pos = ci.transform.position;
        playerSpeedMod = sm;
        triggered = ci.triggered;
    }
}
