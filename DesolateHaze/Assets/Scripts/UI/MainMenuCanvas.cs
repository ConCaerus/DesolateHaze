using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] Button selectOnStart;
    [SerializeField] List<GameObject> backgrounds = new List<GameObject>();

    private void Start() {
        Time.timeScale = 1f;
        selectOnStart.Select();
        ControlSchemeManager.runOnChange += (keyb) => { selectOnStart.Select(); };

        var ind = (int)Saver.getCurArea();
        Debug.Log(Saver.getCurArea().ToString() + " " + ind);
        for(int i = 0; i < backgrounds.Count; i++) 
            backgrounds[i].SetActive(i == ind);
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
