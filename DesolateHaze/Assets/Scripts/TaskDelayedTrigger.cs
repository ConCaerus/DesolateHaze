using UnityEngine;
using UnityEngine.Events;

public class TaskDelayedTrigger : MonoBehaviour {
    [SerializeField] int indexTillEvent;

    [SerializeField] UnityEvent e;


    public void incTask() {
        indexTillEvent--;
        if(indexTillEvent <= 0) {
            e.Invoke();
            Destroy(this);
        }
    }
}
