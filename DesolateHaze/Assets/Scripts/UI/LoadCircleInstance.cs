using UnityEngine;

public class LoadCircleInstance : MonoBehaviour {
    public CheckpointSaveData data;
    public Saver.areaType aType;

    public void runOnHover() {
        LoadCanvas.I.updateInfo(aType);
    }
    public void runOnClick() {
        if(data != null && !data.triggered) return;
        Time.timeScale = 1f;
        if(data != null) {
            Saver.setLastCheckpoint(data.area, data.pos);
            TransitionCanvas.I.loadGame(data.area);
        }
        else TransitionCanvas.I.loadGame(aType);
    }
}
