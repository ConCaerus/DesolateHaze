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
        StartCoroutine(unloader());
    }

    public void loadGame(Saver.areaType a) {
        if(waiter != null) return;
        
        waiter = StartCoroutine(loader(a == Saver.areaType.Outside ? "Game" : 
            a == Saver.areaType.Onsite ? "Onsite" : 
            a == Saver.areaType.Inside ? "Inside" : 
            a == Saver.areaType.Under ? "Under" : 
            a == Saver.areaType.Through ? "Through" : 
            a == Saver.areaType.On ? "On" : 
            a == Saver.areaType.End ? "End" : ""));
    }
    void loadGame() {
        loadGame(CheckpointManager.I.aType);
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
        Time.timeScale = 1f;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        SceneManager.LoadScene(sName);
    }
    IEnumerator unloader() {
        Time.timeScale = 1f;
        background.gameObject.SetActive(true);
        background.color = Color.black;

        //  waits for player to touch ground
        do
            yield return new WaitForFixedUpdate();
        while(PlayerMovement.I != null && !PlayerMovement.I.grounded);
        float blackTime = .75f;
        float elapsedTime = 0f;
        float startTime = Time.realtimeSinceStartup;
        while(elapsedTime < blackTime) {
            float perc = elapsedTime / blackTime;
            background.color = new Color(0f, 0f, 0f, (1f - perc));
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.realtimeSinceStartup - startTime;
            startTime = Time.realtimeSinceStartup;
        }
        background.color = Color.clear;
        yield return new WaitForEndOfFrame();
        background.gameObject.SetActive(false);
    }
}
