using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequenceLoop : MonoBehaviour {
    [SerializeField] List<EventInfo> sequence = new List<EventInfo>();

    [System.Serializable]
    public struct EventInfo {
        public UnityEvent e;
        public List<DelayedEventInfo> delayedEvents;
        public float sequenceCooldown;
    }
    [System.Serializable]
    public struct DelayedEventInfo {
        public UnityEvent e;
        public float delay;
    }

    Coroutine cooldowner = null;

    int curIndex = 0;

    public void triggerNextEvent() {
        if(sequence.Count == 0 || cooldowner != null) return;
        var temp = sequence[curIndex++];
        if(curIndex >= sequence.Count)
            curIndex = 0;
        temp.e.Invoke();
        foreach(var i in temp.delayedEvents)
            StartCoroutine(waiter(i));
        if(temp.sequenceCooldown > 0f)
            cooldowner = StartCoroutine(cooldownWaiter(temp));
    }

    IEnumerator waiter(DelayedEventInfo info) {
        yield return new WaitForSeconds(info.delay);
        info.e.Invoke();
    }
    IEnumerator cooldownWaiter(EventInfo info) {
        yield return new WaitForSeconds(info.sequenceCooldown);
        cooldowner = null;
    }
}
