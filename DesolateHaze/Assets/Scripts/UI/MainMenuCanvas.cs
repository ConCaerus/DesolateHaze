using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] Button selectOnStart;

    private void Start() {
        Time.timeScale = 1f;
        selectOnStart.Select();
        ControlSchemeManager.runOnChange += (keyb) => { selectOnStart.Select(); };
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
