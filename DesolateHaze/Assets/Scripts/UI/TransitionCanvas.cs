using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TransitionCanvas : Singleton<TransitionCanvas> {
    [SerializeField] Image background;

    Coroutine waiter = null;

    private void Start() {
        Time.timeScale = 1.0f;
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
        float blackTime = .35f;
        float elapsedTime = 0f;
        float startTime = Time.realtimeSinceStartup;
        while(elapsedTime < blackTime) {
            float perc = elapsedTime / blackTime;
            background.color = Color.black * perc;
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.realtimeSinceStartup - startTime;
            startTime = Time.realtimeSinceStartup;
        }
        background.color = Color.black;
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(sName);
    }
}
