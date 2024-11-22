using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvas : Singleton<PauseCanvas> {
    [SerializeField] GameObject background;
    [SerializeField] float growSize;

    InputMaster controls;

    public static Action<bool> runOnPauseChange = (bool b) => { };

    bool p = false;
    public bool paused {
        get { return p; }
        set {
            p = value;
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

    public void grow(Transform t) {
        t.localScale = growSize * Vector2.one;
    }
    public void shrink(Transform t) {
        t.localScale = Vector2.one;
    }

    //  buttons
    public void resume() {
        paused = !paused;
    }
    public void mainMenu() {
        TransitionCanvas.I.loadMainMenu();
    }
}
