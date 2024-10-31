using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SneakManagerInstance : MonoBehaviour {
    [SerializeField] float warnTime, failTime;
    [SerializeField] UnityEvent warnEvents = new UnityEvent(), failEvents = new UnityEvent();
    bool hasWarned = false, hasFailed = false;

    [SerializeField] AudioPoolInfo warnSound, failSound;

    float ct = 0f;
    float curTime {
        get { return ct; }
        set { 
            ct = value;
            //  triggers
            if(ct >= warnTime && !hasWarned) {
                hasWarned = true;
                warnEvents.Invoke();
            }
            else if(ct >= failTime && !hasFailed) {
                hasFailed = true;
                failEvents.Invoke();
                enabled = false;
            }

            //  resetters
            else if(hasWarned && ct <= warnTime - 1f)
                hasWarned = false;
        }
    }

    InputMaster controls;
    Coroutine waiter = null;

    private void Start() {
        AudioManager.I.initSound(warnSound);
        AudioManager.I.initSound(failSound);
    }

    private void OnEnable() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => startMoving();
        controls.Player.Move.canceled += ctx => stopMoving();
    }

    private void OnDisable() {
        controls.Disable();
        if(waiter != null) StopCoroutine(waiter);
        waiter = null;
    }

    void startMoving() {
        if(!gameObject.activeInHierarchy) return;
        if(waiter != null) StopCoroutine(waiter);
        waiter = StartCoroutine(moving());
    }
    void stopMoving() {
        if(!gameObject.activeInHierarchy) return;
        if(waiter != null) StopCoroutine(waiter);
        waiter = StartCoroutine(notMoving());
    }

    public void playWarnSound(Transform origin) {
        AudioManager.I.playSound(warnSound, origin.position, 1f);
    }
    public void playFailSound(Transform origin) {
        AudioManager.I.playSound(failSound, origin.position, 1f);
    }

    IEnumerator moving() {
        float st;
        while(true) {
            st = Time.time;
            yield return new WaitForEndOfFrame();
            curTime += Time.time - st;
        }
    }
    IEnumerator notMoving() {
        float st;
        while(true) {
            st = Time.time;
            yield return new WaitForEndOfFrame();
            curTime -= (Time.time - st) * 2f;
        }
    }

    public void testWarn() {
        Debug.Log("Warn");
    }
    public void testFail() {
        Debug.Log("Fail");
    }
}
