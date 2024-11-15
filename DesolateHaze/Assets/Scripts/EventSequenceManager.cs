using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequenceManager : MonoBehaviour {
    [SerializeField] List<UnityEvent> sequence = new List<UnityEvent>();

    public void triggerNextEvent() {
        if(sequence.Count == 0) return;
        sequence[0].Invoke();
        sequence.RemoveAt(0);
    }
}
