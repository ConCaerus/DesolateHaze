using System;
using UnityEngine;

public class PauseCanvas : Singleton<PauseCanvas> {
    [SerializeField] GameObject background;

    InputMaster controls;

    public static Action<bool> runOnPauseChange = (bool b) => { };

    bool p = false;
    public bool paused {
        get { return p; }
        set {
            p = value;
            if(background != null)
                background.SetActive(p);
            Time.timeScale = p ? 0f : 1f;

            runOnPauseChange(p);
        }
    }

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Misc.Pause.performed += ctx => { paused = !paused; };

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
