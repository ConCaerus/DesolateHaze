using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsCanvas : Singleton<OptionsCanvas> {
    [SerializeField] Slider masterVolSlider; // musicVolSlider, sfxVolSlider;
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI screenModeText;
    [SerializeField] Selectable defaultBut;

    FullScreenMode curScreenMode;

    public static System.Action settingsChanged = () => { };

    public bool shown { get; private set; } = false;

    private void Start() {
        setup();
        background.SetActive(false);
        ControlSchemeManager.runOnChange += (keyb) => { if(background.activeInHierarchy && !keyb) defaultBut.Select(); };
    }

    void setup() {
        //  sound
        var master = SaveData.getUniversalFloat("masterVolume", .8f);
        //var music = SaveData.getUniversalFloat("musicVolume", .8f);
        //var sfx = SaveData.getUniversalFloat("sfxVolume", .8f);
        masterVolSlider.value = master;
        //musicVolSlider.value = music;
        //sfxVolSlider.value = sfx;

        //  screen mode
        curScreenMode = (FullScreenMode)SaveData.getUniversalInt("screenMode", (int)FullScreenMode.ExclusiveFullScreen);
        Screen.fullScreenMode = curScreenMode;
        updateScreenModeText();
        settingsChanged();
        StartCoroutine(delaySettingsChanged());
    }

    public void show() {
        shown = true;
        setup();
        background.SetActive(true);
        if(!ControlSchemeManager.I.usingKeyboard) defaultBut.Select();
    }
    public void hide() {
        shown = false;
        background.SetActive(false);
    }

    public bool isOpen() {
        return background.activeInHierarchy;
    }

    public void screenModeToggle(bool right) {
        int cur = (int)curScreenMode;
        cur = right ? cur + 1 : cur - 1;
        if(cur < 0)
            cur = 3;
        else if(cur > 3)
            cur = 0;
        curScreenMode = (FullScreenMode)cur;
        updateScreenModeText();
    }
    void updateScreenModeText() {
        screenModeText.text = curScreenMode == FullScreenMode.ExclusiveFullScreen ? "Fullscreen" : curScreenMode == FullScreenMode.FullScreenWindow ? "Windowed Fullscreen" :
            curScreenMode == FullScreenMode.MaximizedWindow ? "Borderless Window" : "Windowed";
    }
    
    public void apply() {
        //  sound
        SaveData.setUniversalFloat("masterVolume", masterVolSlider.value);
        //SaveData.setUniversalFloat("musicVolume", musicVolSlider.value);
        //SaveData.setUniversalFloat("sfxVolume", sfxVolSlider.value);

        //  screen mode
        SaveData.setUniversalInt("screenMode", (int)curScreenMode);
        Screen.fullScreenMode = curScreenMode;

        settingsChanged();
    }
    public void revert() {
        masterVolSlider.value = .8f;
        //musicVolSlider.value = .8f;
        //sfxVolSlider.value = .8f;
        curScreenMode = FullScreenMode.ExclusiveFullScreen;
        updateScreenModeText();
        apply();
    }

    public float getVolume() {
        return masterVolSlider.value;
    }

    IEnumerator delaySettingsChanged() {
        yield return new WaitForSeconds(.1f);
        settingsChanged();
    }
}
