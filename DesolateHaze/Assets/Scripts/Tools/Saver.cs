using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Saver {
    static string playerTag = "PlayerSaveData";

    public static void savePlayer(PlayerSaveData psd) {
        var data = JsonUtility.ToJson(psd);
        SaveData.setString(playerTag, data);
    }
    public static PlayerSaveData loadPlayer() {
        var data = SaveData.getString(playerTag);
        if(string.IsNullOrEmpty(data))
            return null;
        return JsonUtility.FromJson<PlayerSaveData>(data);
    }
}

[System.Serializable]
public class PlayerSaveData {
    public Vector2 pos;

    public PlayerSaveData(Vector2 p) {
        pos = p;
    }
}
