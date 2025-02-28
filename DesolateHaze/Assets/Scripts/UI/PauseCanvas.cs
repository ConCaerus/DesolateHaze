using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : Singleton<PauseCanvas> {
    [SerializeField] GameObject background;
    [SerializeField] Button defaultBut;

    InputMaster controls;

    public static System.Action<bool> runOnPauseChange = (bool b) => { };

    bool p = false;
    public bool paused {
        get { return p; }
        set {
            p = value;
            if(background != null)
                background.SetActive(p);
            if(p && !ControlSchemeManager.I.usingKeyboard) defaultBut.Select();
            Time.timeScale = p ? 0f : 1f;

            runOnPauseChange(p);
        }
    }

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Misc.Pause.performed += ctx => { paused = !paused; };

        ControlSchemeManager.runOnChange += (keyb) => {
            if(!paused) return;
            if(!keyb) defaultBut.Select();
        };

        paused = false;
    }

    private void OnDisable() {
        runOnPauseChange = (bool b) => { };
    }

    //  buttons
    public void resume() {
        paused = !paused;
    }
    public void mainMenu() {
        TransitionCanvas.I.loadMainMenu();
    }
}
