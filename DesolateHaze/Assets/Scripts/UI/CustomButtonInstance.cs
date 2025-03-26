using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CustomButtonInformation))]
public class CustomButtonInstance : Button {
    CustomButtonInformation cbi;

    protected override void Start() {
        base.Start();
        if(!TryGetComponent(out cbi)) {
            Debug.LogError("Attatch CustomButtonInformation script to " + gameObject.name);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        hover();
    }
    public override void OnSelect(BaseEventData eventData) {
        hover();
    }

    public override void OnPointerExit(PointerEventData eventData) {
        dehover();
    }
    public override void OnDeselect(BaseEventData eventData) {
        dehover();
    }

    public override void OnPointerClick(PointerEventData eventData) {
        click();
    }
    public override void OnSubmit(BaseEventData eventData) {
        click();
    }
    

    void hover() {
        if(!gameObject.activeInHierarchy) return;
        if(cbi.playSound) ButtonSoundPlayer.I.playButtonSound();  //  sound
        //if(cbi.showEffect) UIIconCanvas.I.show(transform); //  show effect
        ConTween.stopTransTween(transform);
        ConTween.tweenScale(transform, cbi.hoverSize, new CTweenInfo(.15f, false));

        cbi.hoverEvents.Invoke();
    }
    void dehover() {
        //if(cbi.showEffect) UIIconCanvas.I.hide();  //  hide effect
        ConTween.stopTransTween(transform);
        ConTween.tweenScale(transform, cbi.normalSize, new CTweenInfo(.25f, false));

        cbi.dehoverEvents.Invoke();
    }
    void click() {
        if(cbi.playSound) ButtonSoundPlayer.I.playClickSound();   //  sound
        //if(cbi.showEffect) UIIconCanvas.I.hardHide();
        ConTween.stopTransTween(transform);
        ConTween.tweenScale(transform, cbi.normalSize, new CTweenInfo(.25f, false));

        cbi.clickEvents.Invoke();
    }
}
