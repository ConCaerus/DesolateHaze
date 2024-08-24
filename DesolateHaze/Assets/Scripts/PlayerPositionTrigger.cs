using UnityEngine;
using UnityEngine.Events;

public class PlayerPositionTrigger : MonoBehaviour {
    [SerializeField] UnityEvent events;
    [SerializeField] bool singleUse = true;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Player") {
            events.Invoke();
            if(singleUse)
                enabled = false;
        }
    }
}
