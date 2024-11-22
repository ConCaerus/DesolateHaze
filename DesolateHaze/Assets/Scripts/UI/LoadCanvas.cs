using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadCanvas : Singleton<LoadCanvas> {
    [SerializeField] GameObject loaderPref;
    [SerializeField] Transform holder;
    [SerializeField] TextMeshProUGUI aText, titleText;
    [SerializeField] Transform background;

    List<Transform> loaders = new List<Transform>();


    private void Start() {
        spawnLoaders();
    }

    void spawnLoaders() {
        destroyLoaders();
        for(int i = 0; i < Saver.getAreaCount(); i++) {
            for(int j = 0; j < Saver.getCheckpointCount((Saver.areaType)i + 1); j++) {
                var data = Saver.getCheckpointAtIndex((Saver.areaType)i + 1, j);
                var temp = Instantiate(loaderPref, holder);
                temp.name = ((Saver.areaType)i + 1).ToString();
                var c = CheckpointManager.I.getColor((Saver.areaType)i + 1);
                temp.GetComponent<Image>().color = data.triggered ? c : (c + Color.black * 2) / 3f;
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

    public void updateInfo(CheckpointSaveData data) {
        aText.text = "<alpha=#00>" + titleText.text + "<alpha=#FF>: " + data.area.ToString();
    }

    public void show() {
        background.gameObject.SetActive(true);
        spawnLoaders();
    }
    public void hide() {
        background.gameObject.SetActive(false);
        destroyLoaders();
    }
}
