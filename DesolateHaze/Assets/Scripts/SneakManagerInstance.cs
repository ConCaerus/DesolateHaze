using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SneakManagerInstance : MonoBehaviour {
    [SerializeField] float warnTime, failTime;
    [SerializeField] UnityEvent warnEvents = new UnityEvent(), failEvents = new UnityEvent();
    bool hasWarned = false, hasFailed = false;

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
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => {
            if(waiter != null) StopCoroutine(waiter);
            waiter = StartCoroutine(moving());
        };
        controls.Player.Move.canceled += ctx => {
            if(waiter != null) StopCoroutine(waiter);
            waiter = StartCoroutine(notMoving());
        };
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
            curTime -= Time.time - st;
        }
    }

    public void testWarn() {
        Debug.Log("Warn");
    }
    public void testFail() {
        Debug.Log("Fail");
    }
}
