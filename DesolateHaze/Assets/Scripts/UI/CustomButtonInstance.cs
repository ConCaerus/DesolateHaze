using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CustomButtonInformation))]
public class CustomButtonInstance : Button {
    CustomButtonInformation cbi;
    Coroutine scaler = null;

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
        if(scaler != null) StopCoroutine(scaler);
        scaler = StartCoroutine(scale(cbi.hoverSize, .25f));
        if(cbi.playSound) ButtonSoundPlayer.I.playButtonSound();  //  sound
        //if(cbi.showEffect) UIIconCanvas.I.show(transform); //  show effect

        cbi.hoverEvents.Invoke();
    }
    void dehover() {
        //if(cbi.showEffect) UIIconCanvas.I.hide();  //  hide effect
        if(scaler != null) StopCoroutine(scaler);
        scaler = StartCoroutine(scale(cbi.normalSize, .25f));

        cbi.dehoverEvents.Invoke();
    }
    void click() {
        if(cbi.playSound) ButtonSoundPlayer.I.playClickSound();   //  sound
        //if(cbi.showEffect) UIIconCanvas.I.hardHide();
        if(scaler != null) StopCoroutine(scaler);
        scaler = StartCoroutine(scale(cbi.normalSize, .25f));

        cbi.clickEvents.Invoke();
    }

    IEnumerator scale(float endSize, float dur) {
        var startTime = Time.realtimeSinceStartup;
        var startSize = transform.localScale.x;
        var sizeOffset = endSize - startSize;
        var elapsedTime = 0f;

        while(elapsedTime < dur) {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.realtimeSinceStartup - startTime;
            startTime = Time.realtimeSinceStartup;
            transform.localScale = Vector3.one * (startSize + sizeOffset * (elapsedTime / dur));
        }

        transform.localScale = Vector3.one * endSize;
        scaler = null;
    }
}
