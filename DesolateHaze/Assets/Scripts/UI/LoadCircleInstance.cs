using UnityEngine;

public class LoadCircleInstance : MonoBehaviour {
    public CheckpointSaveData data;

    public void runOnHover() {
        LoadCanvas.I.updateInfo(data);
    }
    public void runOnClick() {
        if(!data.triggered) return;
        Time.timeScale = 1f;
        Saver.setLastCheckpoint(data.area, data.pos);
        TransitionCanvas.I.loadGame(data.area);
    }
}
