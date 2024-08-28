using UnityEngine;
using DG.Tweening;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] float growSize;

    private void Start() {
        DOTween.Init();
    }

    public void grow(Transform t) {
        t.DOKill();
        t.DOScale(growSize, .15f);
    }
    public void shrink(Transform t) {
        t.DOKill();
        t.DOScale(1f, .25f);
    }

    //  buttons
    public void playNewGame() {
        Saver.clearSave();
        TransitionCanvas.I.loadGame();
    }
    public void playContinue() {
        TransitionCanvas.I.loadGame();
    }
    public void load() {
        TransitionCanvas.I.loadGame();
    }
    public void quit() {
        Application.Quit();
    }
}
