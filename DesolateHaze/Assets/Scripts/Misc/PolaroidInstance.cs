using System.Collections;
using UnityEngine;

public class PolaroidInstance : MonoBehaviour {
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;

    Coroutine checker = null;

    public void init() {
        if(checker == null) checker = StartCoroutine(waiter());
    }

    IEnumerator waiter() {
        do
            yield return new WaitForFixedUpdate();
        while(rb.linearVelocity.magnitude > 0f);

        Destroy(rb);
        Destroy(col);
        Destroy(this);
    }
}
