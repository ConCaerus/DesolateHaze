using DG.Tweening;
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
        transform.DOComplete();
        transform.DOScale(cbi.hoverSize, .15f);
        if(cbi.playSound) ButtonSoundPlayer.I.playButtonSound();  //  sound
        //if(cbi.showEffect) UIIconCanvas.I.show(transform); //  show effect

        cbi.hoverEvents.Invoke();
    }
    void dehover() {
        //if(cbi.showEffect) UIIconCanvas.I.hide();  //  hide effect
        transform.DOComplete();
        transform.DOScale(cbi.normalSize, .25f);

        cbi.dehoverEvents.Invoke();
    }
    void click() {
        if(cbi.playSound) ButtonSoundPlayer.I.playClickSound();   //  sound
        //if(cbi.showEffect) UIIconCanvas.I.hardHide();
        transform.DOComplete();
        transform.DOScale(cbi.normalSize, .25f);

        cbi.clickEvents.Invoke();
    }
}
