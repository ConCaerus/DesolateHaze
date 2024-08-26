using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TransitionCanvas : Singleton<TransitionCanvas> {
    [SerializeField] Image background;

    Coroutine waiter = null;

    private void Start() {
        DOTween.Init();
        background.gameObject.SetActive(true);
        background.color = Color.black;
        background.DOColor(Color.clear, .25f).OnComplete(() => { background.gameObject.SetActive(false); });
    }

    public void loadGame() {
        if(waiter != null) return;
        waiter = StartCoroutine(loader("Game"));
    }
    public void loadMainMenu() {
        if(waiter != null) return;
        waiter = StartCoroutine(loader("MainMenu"));
    }

    public void loadGameAfterDeath(float time) {
        Invoke("loadGame", time);
    }

    IEnumerator loader(string sName) {
        background.gameObject.SetActive(true);
        background.color = Color.clear;
        background.DOColor(Color.black, .35f);
        yield return new WaitForSeconds(.35f);
        SceneManager.LoadScene(sName);
    }
}
