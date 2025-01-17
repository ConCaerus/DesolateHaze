using UnityEngine;

public class MainMenuCanvas : MonoBehaviour {
    private void Start() {
        Time.timeScale = 1f;
    }

    //  buttons
    public void playNewGame() {
        Saver.clearSave();
        TransitionCanvas.I.loadGame(Saver.areaType.Outside);
    }
    public void playContinue() {
        TransitionCanvas.I.loadGame(Saver.getCurArea());
    }
    public void quit() {
        Application.Quit();
    }
}
