using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : Singleton<LoadCanvas> {
    [SerializeField] GameObject loaderPref;
    [SerializeField] Transform holder;
    [SerializeField] TextMeshProUGUI aText, titleText;
    [SerializeField] Transform background;
    [SerializeField] bool demo = false;

    List<Transform> loaders = new List<Transform>();

    public bool shown { get; private set; } = false;


    private void Start() {
        spawnLoaders();
        ControlSchemeManager.runOnChange += (keyb) => { if(!keyb && loaders.Count > 0 && shown) loaders[0].GetComponent<Selectable>().Select(); };
    }

    void spawnLoaders() {
        destroyLoaders();
        for(int i = 0; i < Saver.getAreaCount(); i++) {
            if(i > 0 && demo) break;
            //  no save data for area, so create a default one
            if(Saver.getCheckpointCount((Saver.areaType)i + 1) == 0) {
                var temp = Instantiate(loaderPref, holder);
                temp.name = ((Saver.areaType)i + 1).ToString();
                var c = CheckpointManager.I.getColor((Saver.areaType)i + 1);
                temp.GetComponent<Image>().color = c;
                temp.GetComponent<LoadCircleInstance>().aType = ((Saver.areaType)i + 1);
                temp.GetComponent<LoadCircleInstance>().data = null;
                loaders.Add(temp.transform);
            }

            for(int j = 0; j < Saver.getCheckpointCount((Saver.areaType)i + 1); j++) {
                var data = Saver.getCheckpointAtIndex((Saver.areaType)i + 1, j);
                var temp = Instantiate(loaderPref, holder);
                temp.name = ((Saver.areaType)i + 1).ToString();
                var c = CheckpointManager.I.getColor((Saver.areaType)i + 1);
                temp.GetComponent<Image>().color = data.triggered ? c : (c + Color.black * 2) / 3f;
                temp.GetComponent<Button>().interactable = data.triggered;
                temp.GetComponent<LoadCircleInstance>().data = data;
                loaders.Add(temp.transform);
            }
        }
    }
    void destroyLoaders() {
        for(int i = loaders.Count - 1; i >= 0; i--)
            Destroy(loaders[i].gameObject);
        loaders.Clear();
    }

    public void updateInfo(Saver.areaType area) {
        aText.text = "<alpha=#00>" + titleText.text + "<alpha=#FF>: " + area.ToString();
    }

    public void show() {
        shown = true;
        background.gameObject.SetActive(true);
        spawnLoaders();
        if(!ControlSchemeManager.I.usingKeyboard) loaders[0].GetComponent<Selectable>().Select();
    }
    public void hide() {
        shown = false;
        background.gameObject.SetActive(false);
        destroyLoaders();
    }
}
